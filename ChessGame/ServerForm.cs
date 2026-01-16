using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChessGame
{
    public partial class ServerForm : Form
    {
        private bool serverRunning = false;
        private int connectedClients = 0;

        private System.Windows.Forms.Timer countdownTimer;
        private int countdownSeconds;

        private bool matchStarted = false;

        private TcpServer tcpServer;

        private Dictionary<string, string> clientList = new Dictionary<string, string>();

        public ServerForm()
        {
            InitializeComponent();
            UpdateUI();
        }

        void AddClient(string playerName, string clientIp)
        {
            string displayName = playerName.Contains("(") ? playerName.Substring(0, playerName.IndexOf("(")).Trim() : playerName;
            string clientKey = displayName;

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            string displayText = $"[{timestamp}] {displayName} connected from {clientIp}";

            lock (clientList)
            {
                if (!clientList.ContainsKey(clientKey))
                {
                    clientList[clientKey] = clientIp;
                    msgServer.Items.Add(displayText);
                    connectedClients++;
                    UpdateUI();
                }
            }
        }

        void RemoveClient(string clientInfo)
        {
            string playerName = clientInfo.Contains("(") ? clientInfo.Substring(0, clientInfo.IndexOf("(")).Trim() : clientInfo;

            lock (clientList)
            {
                if (clientList.ContainsKey(playerName))
                {
                    clientList.Remove(playerName);

                    string timestamp = DateTime.Now.ToString("HH:mm:ss");
                    string displayText = $"[{timestamp}] {playerName} disconnected";
                    msgServer.Items.Add(displayText);

                    connectedClients--;
                    UpdateUI();
                }
            }
        }

        private void UpdateUI()
        {
            if (serverRunning)
            {
                lblServerStatus.Text = "Server status: RUNNING";
                lblServerStatus.ForeColor = Color.Green;
                lblServerStatus.BackColor = Color.White;
                btnStartServer.Enabled = false;
                btnEndServer.Enabled = true;
            }
            else
            {
                lblServerStatus.Text = "Server status: STOPPED";
                lblServerStatus.ForeColor = Color.Red;
                lblServerStatus.BackColor = Color.White;
                btnStartServer.Enabled = true;
                btnEndServer.Enabled = false;
            }

            lblConnectClient.Text = $"Connected clients: {connectedClients} / 2";
            btnStartMatch.Enabled = serverRunning && connectedClients == 2 && !matchStarted && (countdownTimer == null || !countdownTimer.Enabled);
            btnStopMatch.Enabled = matchStarted;
        }

        private void btnStopMatch_Click(object sender, EventArgs e)
        {
            if (!matchStarted) return;

            tcpServer?.BroadcastCountdown("[STOPMATCH]");
            System.Threading.Thread.Sleep(100);

            matchStarted = false;
            lblMatchStatus.Text = "Ván đấu đã kết thúc";
            msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Match stopped by server");
            UpdateUI();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            serverRunning = true;
            matchStarted = false;
            connectedClients = 0;
            clientList.Clear();
            msgServer.Items.Clear();
            lblMatchStatus.Text = "Waiting for players...";

            tcpServer = new TcpServer();
            tcpServer.OnClientConnected += TcpServer_OnClientConnected;
            tcpServer.OnClientDisconnected += TcpServer_OnClientDisconnected;
            tcpServer.OnLogMessage += TcpServer_OnLogMessage;
            tcpServer.OnMatchShouldReset += TcpServer_OnMatchShouldReset;
            tcpServer.OnClientPaused += TcpServer_OnClientPaused;
            tcpServer.OnClientResumed += TcpServer_OnClientResumed;
            tcpServer.OnClientExited += TcpServer_OnClientExited;
            tcpServer.OnMatchEnded += TcpServer_OnMatchEnded;
            tcpServer.Start();

            msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Server started");
            UpdateUI();
        }

        private void btnEndServer_Click(object sender, EventArgs e)
        {
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            if (matchStarted && tcpServer != null)
            {
                tcpServer.BroadcastCountdown("[STOPMATCH]");
                System.Threading.Thread.Sleep(100);
            }

            if (tcpServer != null)
            {
                tcpServer.Stop();
                tcpServer.OnClientConnected -= TcpServer_OnClientConnected;
                tcpServer.OnClientDisconnected -= TcpServer_OnClientDisconnected;
                tcpServer.OnLogMessage -= TcpServer_OnLogMessage;
                tcpServer.OnMatchShouldReset -= TcpServer_OnMatchShouldReset;
                tcpServer.OnClientPaused -= TcpServer_OnClientPaused;
                tcpServer.OnClientResumed -= TcpServer_OnClientResumed;
                tcpServer.OnClientExited -= TcpServer_OnClientExited;
                tcpServer.OnMatchEnded -= TcpServer_OnMatchEnded;
                tcpServer = null;
            }

            serverRunning = false;
            matchStarted = false;
            connectedClients = 0;
            clientList.Clear();
            msgServer.Items.Clear();
            msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Server stopped");

            lblMatchStatus.Text = "Server stopped";
            UpdateUI();
        }

        private void StartCountdown(int seconds)
        {
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            countdownSeconds = seconds;
            lblMatchStatus.Text = $"Match starts in {countdownSeconds} seconds...";

            tcpServer?.BroadcastCountdown($"Match starts in {countdownSeconds} seconds...");
            msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Match countdown started");

            countdownTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            countdownTimer.Tick += (s, ev) =>
            {
                countdownSeconds--;
                if (countdownSeconds > 0)
                {
                    lblMatchStatus.Text = $"Match starts in {countdownSeconds} seconds...";
                    tcpServer?.BroadcastCountdown($"Match starts in {countdownSeconds} seconds...");
                }
                else
                {
                    countdownTimer.Stop();
                    countdownTimer.Dispose();
                    countdownTimer = null;

                    matchStarted = true;
                    lblMatchStatus.Text = "Match started!";

                    tcpServer?.BroadcastCountdown("Match started!");
                    tcpServer?.StartMatch();

                    msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Match started!");
                    UpdateUI();
                }
            };

            countdownTimer.Start();
            UpdateUI();
        }

        private void btnStartMatch_Click(object sender, EventArgs e)
        {
            if (connectedClients == 2 && serverRunning && !matchStarted && (countdownTimer == null || !countdownTimer.Enabled))
            {
                btnStartMatch.Enabled = false;
                UpdateUI();
                StartCountdown(5);
            }
        }

        private void TcpServer_OnClientConnected(string clientInfo)
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnClientConnected(clientInfo))); return; }

            string playerName = clientInfo.Contains("(") ? clientInfo.Substring(0, clientInfo.IndexOf("(")).Trim() : clientInfo;
            string clientIp = clientInfo.Contains("(") ? clientInfo.Substring(clientInfo.IndexOf("(") + 1, clientInfo.IndexOf(")") - clientInfo.IndexOf("(") - 1) : "";

            AddClient(playerName, clientIp);
        }

        private void TcpServer_OnClientDisconnected(string clientInfo)
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnClientDisconnected(clientInfo))); return; }
            RemoveClient(clientInfo);
        }

        private void TcpServer_OnLogMessage(string message)
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnLogMessage(message))); return; }
            System.Diagnostics.Debug.WriteLine($"[TCP Server] {message}");
        }

        private void TcpServer_OnMatchShouldReset()
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnMatchShouldReset())); return; }

            if (matchStarted)
            {
                matchStarted = false;
                lblMatchStatus.Text = "Waiting for players...";
                msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Match auto-reset: not enough players");
                UpdateUI();
            }
        }

        private void TcpServer_OnClientPaused(string playerName, string timestamp)
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnClientPaused(playerName, timestamp))); return; }

            lblMatchStatus.Text = $"Match is paused by {playerName}";
            msgServer.Items.Add($"[{timestamp}] {playerName} đã tạm dừng ván đấu");
        }

        private void TcpServer_OnClientResumed(string playerName, string timestamp)
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnClientResumed(playerName, timestamp))); return; }

            if (matchStarted)
            {
                lblMatchStatus.Text = "Match started!";
            }
            msgServer.Items.Add($"[{timestamp}] {playerName} đã tiếp tục ván đấu");
        }

        private void TcpServer_OnClientExited(string playerName, string timestamp)
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnClientExited(playerName, timestamp))); return; }
            msgServer.Items.Add($"[{timestamp}] {playerName} đã thoát ván đấu");
        }

        private void TcpServer_OnMatchEnded()
        {
            if (InvokeRequired) { Invoke(new Action(() => TcpServer_OnMatchEnded())); return; }

            if (matchStarted)
            {
                matchStarted = false;
                lblMatchStatus.Text = "Ván đấu đã kết thúc";
                msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Ván đấu đã kết thúc do người chơi thoát");
                UpdateUI();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // No-op
        }
    }
}
