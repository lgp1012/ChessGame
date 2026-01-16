using System;
using System.Drawing;
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
            // Sử dụng BeginInvoke để không bị block khi ShowDialog đang chạy
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => TcpConnection_OnMessageReceived(message)));
                return;
            }

            UpdateUI($"[Server] {message}");

            if (message.Contains("Match started!"))
            {
                // wait for opponent info
            }
            else if (message.StartsWith("[MOVE]"))
            {
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.HandleOpponentMove(message);
                }
            }
            else if (message.StartsWith("[TURN]"))
            {
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.SetTurn(true);
                }
            }
            else if (message.StartsWith("[OPPONENT]"))
            {
                string[] parts = message.Split('|');
                if (parts.Length >= 3)
                {
                    opponentName = parts[1].Trim();
                    playerColor = parts[2].Trim() == "WHITE" ? PieceColor.White : PieceColor.Black;
                    StartChessGame();
                }
            }
            else if (message.StartsWith("[PAUSE]"))
            {
                string opponentPausedName = message.Substring(7).Trim();
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.ShowOpponentPauseMessage(opponentPausedName);
                }
            }
            else if (message.StartsWith("[RESUME]"))
            {
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.HideOpponentPauseOverlay();
                }
            }
            else if (message.StartsWith("[EXIT]"))
            {
                string opponentExitName = message.Substring(6).Trim();
                if (gameForm != null && !gameForm.IsDisposed)
                {
                    gameForm.DisableExitNotification();
                    MessageBox.Show($"{opponentExitName} đã thoát ván đấu.\nVán đấu kết thúc!",
                        "Đối thủ thoát", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    gameForm.Close();
                }
                inGame = false;
                this.Show();
            }
            else if (message.Contains("[STOPMATCH]"))
            {
                HandleServerStopMatch();
            }
            else if (message.Contains("Server is shutting down"))
            {
                HandleServerStopMatch();
            }
        }

        private void HandleServerStopMatch()
        {
            // Xử lý trực tiếp trên gameForm nếu đang mở
            if (gameForm != null && !gameForm.IsDisposed && inGame)
            {
                // Gọi method trên gameForm - nó sẽ tự xử lý InvokeRequired
                gameForm.GameStoppedByServer();
            }
            else
            {
                inGame = false;
                this.Show();
            }
        }

        private void StartChessGame()
        {
            if (inGame) return;

            inGame = true;
            this.Hide();

            gameForm = new ChessGameForm(playerColor, playerName, opponentName);
            gameForm.OnGameMessage += (msg) => tcpConnection.SendMessage(msg);
            gameForm.OnGameExited += () =>
            {
                inGame = false;
                if (!this.IsDisposed)
                {
                    this.Show();
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
                gameForm.DisableExitNotification();
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
