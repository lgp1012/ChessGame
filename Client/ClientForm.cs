using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    public partial class ClientForm : Form
    {
        private TcpClientConnection tcpConnection;
        private string playerName;
        private string serverIP;
        private PieceColor playerColor = PieceColor.White;
        private string opponentName = "";
        private ChessGameForm gameForm;
        private bool inGame = false;
        private UdpGameClient udpClient;

        public ClientForm(string name, string ip)
        {
            InitializeComponent();
            playerName = name;
            serverIP = ip;
            
            if (!string.IsNullOrEmpty(playerName))
            {
                this.Text = $"Chess Game - {playerName}";
                lblTitle.Text = $"Chess Game Client - {playerName}";
            }

            tcpConnection = new TcpClientConnection(serverIP);
            tcpConnection.OnMessageReceived += TcpConnection_OnMessageReceived;
            tcpConnection.OnConnectionStatusChanged += TcpConnection_OnConnectionStatusChanged;
            tcpConnection.OnError += TcpConnection_OnError;

            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
        }

        private void TcpConnection_OnMessageReceived(string message)
        {
            // Xử lý trên UI thread
            if (InvokeRequired)
            {
                Invoke(new Action(() => TcpConnection_OnMessageReceived(message)));
                return;
            }

            UpdateUI($"[Server] {message}");

            // Match started
            if (message.Contains("Match started!"))
            {
                // Khởi tạo UDP client và gửi port cho server
                InitializeUdpClient();
            }
            // UDP info từ server
            else if (message.StartsWith("[UDP_INFO]"))
            {
                string[] parts = message.Substring(10).Split('|');
                if (parts.Length >= 2)
                {
                    string opponentIp = parts[0].Trim();
                    int opponentPort = int.Parse(parts[1].Trim());
                    
                    if (udpClient != null)
                    {
                        udpClient.ConnectToOpponent(opponentIp, opponentPort);
                        UpdateUI($"[UDP] Connected to opponent at {opponentIp}:{opponentPort}");
                    }
                    
                    // Bắt đầu game sau khi kết nối UDP
                    StartChessGame();
                }
            }
            // Opponent info
            else if (message.StartsWith("[OPPONENT]"))
            {
                string[] parts = message.Split('|');
                if (parts.Length >= 3)
                {
                    opponentName = parts[1].Trim();
                    playerColor = parts[2].Trim() == "WHITE" ? PieceColor.White : PieceColor.Black;
                }
            }
            // Opponent paused
            else if (message.StartsWith("[PAUSE]"))
            {
                string opponentPausedName = message.Substring(7).Trim();
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.ShowOpponentPauseMessage(opponentPausedName);
                }
            }
            // Opponent resumed
            else if (message.StartsWith("[RESUME]"))
            {
                string opponentResumedName = message.Substring(8).Trim();
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.HideOpponentPauseOverlay();
                }
            }
            // Opponent exited
            else if (message.StartsWith("[EXIT]"))
            {
                string opponentExitName = message.Substring(6).Trim();
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"{opponentExitName} đã thoát ván đấu. Ván đấu kết thúc!", 
                            "Đối thủ thoát", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        gameForm.Close();
                    }));
                }
                inGame = false;
                
                // Cleanup UDP client
                if (udpClient != null)
                {
                    udpClient.Stop();
                    udpClient = null;
                }
            }
            // Server stopped match - PHẢI XỬ LÝ ĐÚNG
            else if (message.Contains("[STOPMATCH]"))
            {
                HandleServerStopMatch();
            }
            // Server shutdown
            else if (message.Contains("Server is shutting down"))
            {
                HandleServerStopMatch();
            }
        }

        private void InitializeUdpClient()
        {
            try
            {
                // Khởi tạo UDP client với port động (0 = tự động chọn)
                udpClient = new UdpGameClient();
                udpClient.Start(0); // Port 0 để OS tự động chọn port khả dụng
                
                // Lấy port được gán
                int localPort = udpClient.GetLocalPort();
                
                // Gửi UDP port cho server
                tcpConnection.SendMessage($"[UDP_PORT]{localPort}");
                UpdateUI($"[UDP] Initialized on port {localPort}");
            }
            catch (Exception ex)
            {
                UpdateUI($"[UDP] Error initializing: {ex.Message}");
            }
        }

        private void HandleServerStopMatch()
        {
            if (gameForm != null && !gameForm.IsDisposed && inGame)
            {
                // Đóng game form và quay về
                gameForm.Invoke(new Action(() =>
                {
                    MessageBox.Show("Server đã dừng trận đấu!", "Match Stopped",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    gameForm.Close();
                }));
            }
            inGame = false;
            
            // Cleanup UDP client
            if (udpClient != null)
            {
                udpClient.Stop();
                udpClient = null;
            }
            
            // Show lại form kết nối
            if (!this.Visible)
            {
                this.Show();
            }
        }

        private void StartChessGame()
        {
            if (inGame) return;

            inGame = true;
            this.Hide();

            gameForm = new ChessGameForm(playerColor, playerName, opponentName, udpClient);
            gameForm.OnGameMessage += (msg) => tcpConnection.SendMessage(msg);
            gameForm.OnGameExited += () =>
            {
                inGame = false;
                if (!this.IsDisposed)
                {
                    this.Show();
                }
                
                // Dọn dẹp UDP client
                if (udpClient != null)
                {
                    udpClient.Stop();
                    udpClient = null;
                }
                
                gameForm = null;
            };

            gameForm.ShowDialog();
        }

        private void TcpConnection_OnConnectionStatusChanged(string status)
        {
            UpdateUI($"Connection status: {status}");
        }

        private void TcpConnection_OnError(string errorMessage)
        {
            UpdateUI($"Error: {errorMessage}");
        }

        private async void ConnectToServer()
        {
            if (!tcpConnection.IsConnected)
            {
                btnConnect.Enabled = false;
                await tcpConnection.ConnectAsync(playerName);
            }
        }

        private void SendMessageToServer(string message)
        {
            tcpConnection.SendMessage(message);
            UpdateUI($"You: {message}");
        }

        private void DisconnectFromServer()
        {
            if (inGame && gameForm != null && !gameForm.IsDisposed)
            {
                gameForm.Close();
            }
            tcpConnection.Disconnect();
        }

        private void UpdateUI(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUI(message)));
                return;
            }

            bool connected = tcpConnection != null && tcpConnection.IsConnected;
            lblStatus.Text = $"Status: {(connected ? "Connected" : "Disconnected")}";
            lblStatus.ForeColor = connected ? Color.Green : Color.Red;

            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;

            lstMessages.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstMessages.TopIndex = lstMessages.Items.Count - 1;
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            if (!tcpConnection.IsConnected)
            {
                ConnectToServer();
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            if (tcpConnection.IsConnected)
            {
                DisconnectFromServer();
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtMessage.Text))
            {
                SendMessageToServer(txtMessage.Text);
                txtMessage.Clear();
                txtMessage.Focus();
            }
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectFromServer();
        }
    }
}
