using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGame
{
    public partial class ServerForm : Form
    {
        private bool serverRunning = false;
        private int connectedClients = 0;

        private System.Windows.Forms.Timer countdownTimer;
        private int countdownSeconds;

        // New flag: true when a match has actually started (after countdown completes)
        private bool matchStarted = false;

        // TCP Server instance
        private TcpServer tcpServer;

        // Dictionary to track clients by name
        private Dictionary<string, string> clientList = new Dictionary<string, string>();

        public ServerForm()
        {
            InitializeComponent();
            UpdateUI();
        }

        void AddClient(string playerName, string clientIp)
        {
            // Extract just the player name (remove IP part if it exists)
            string displayName = playerName.Contains("(") ? playerName.Substring(0, playerName.IndexOf("(")).Trim() : playerName;
            string clientKey = displayName;

            // Add to dictionary and listbox with timestamp and IP
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
            // Extract just the player name (remove IP part if it exists)
            string playerName = clientInfo.Contains("(") ? clientInfo.Substring(0, clientInfo.IndexOf("(")).Trim() : clientInfo;

            lock (clientList)
            {
                if (clientList.ContainsKey(playerName))
                {
                    clientList.Remove(playerName);
                    
                    // Add disconnect message to msgServer
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
            // Update server status with color
            if (serverRunning)
            {
                lblServerStatus.Text = "Server status: RUNNING";
                lblServerStatus.ForeColor = Color.Green;  // Green when running
                lblServerStatus.BackColor = Color.White;
                btnStartServer.Enabled = false;  // Disable Start button when running
                btnEndServer.Enabled = true;     // Enable End button when running
            }
            else
            {
                lblServerStatus.Text = "Server status: STOPPED";
                lblServerStatus.ForeColor = Color.Red;    // Red when stopped
                lblServerStatus.BackColor = Color.White;
                btnStartServer.Enabled = true;   // Enable Start button when stopped
                btnEndServer.Enabled = false;    // Disable End button when stopped
            }

            lblConnectClient.Text = $"Connected clients: {connectedClients} / 2";

            btnStartMatch.Enabled = serverRunning && connectedClients == 2 && (countdownTimer == null || !countdownTimer.Enabled);

            btnStopMatch.Enabled = matchStarted;
        }

        private void btnStopMatch_Click(object sender, EventArgs e)
        {
            if (!matchStarted)
            {
                return;
            }

            matchStarted = false;
            lblMatchStatus.Text = "Match paused by server";
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

            // Start the TCP server to accept client connections
            tcpServer = new TcpServer();
            tcpServer.OnClientConnected += TcpServer_OnClientConnected;
            tcpServer.OnClientDisconnected += TcpServer_OnClientDisconnected;
            tcpServer.OnLogMessage += TcpServer_OnLogMessage;
            tcpServer.Start();

            // Add server started message
            msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Server started");

            UpdateUI();
        }

        private void btnEndServer_Click(object sender, EventArgs e)
        {
            // If a countdown timer is running, stop and dispose it
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            // Stop the TCP server
            if (tcpServer != null)
            {
                tcpServer.Stop();
                tcpServer.OnClientConnected -= TcpServer_OnClientConnected;
                tcpServer.OnClientDisconnected -= TcpServer_OnClientDisconnected;
                tcpServer.OnLogMessage -= TcpServer_OnLogMessage;
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
            // Ensure that any previous timer is cleared
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            // Starting a countdown implies the match is not yet started
            matchStarted = false;

            countdownSeconds = seconds;
            lblMatchStatus.Text = $"Match starts in {countdownSeconds} seconds...";
            
            // Broadcast countdown to clients
            if (tcpServer != null)
            {
                tcpServer.BroadcastCountdown($"Match starts in {countdownSeconds} seconds...");
            }

            // Log to msgServer
            msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Match countdown started");

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000; // 1 second
            countdownTimer.Tick += (s, ev) =>
            {
                countdownSeconds--;
                if (countdownSeconds > 0)
                {
                    lblMatchStatus.Text = $"Match starts in {countdownSeconds} seconds...";
                    // Send updated countdown to clients
                    if (tcpServer != null)
                    {
                        tcpServer.BroadcastCountdown($"Match starts in {countdownSeconds} seconds...");
                    }
                }
                else
                {
                    // Countdown finished -> mark match as started and clear timer
                    countdownTimer.Stop();
                    countdownTimer.Dispose();
                    countdownTimer = null;

                    matchStarted = true;
                    lblMatchStatus.Text = "Match started!";
                    
                    // Send match started message to clients
                    if (tcpServer != null)
                    {
                        tcpServer.BroadcastCountdown("Match started!");
                    }

                    msgServer.Items.Add($"[{DateTime.Now:HH:mm:ss}] Match started!");
                    
                    UpdateUI();
                }
            };

            countdownTimer.Start();
            UpdateUI();
        }

        private void btnStartMatch_Click(object sender, EventArgs e)
        {
            if (connectedClients == 2 && serverRunning && (countdownTimer == null || !countdownTimer.Enabled))
            {
                btnStartMatch.Enabled = false;
                UpdateUI();

                StartCountdown(5);
            }
        }

        // Event handlers for TCP server
        private void TcpServer_OnClientConnected(string clientInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => TcpServer_OnClientConnected(clientInfo)));
                return;
            }

            // Extract player name and IP
            // clientInfo format: "PlayerName (IP)"
            string playerName = clientInfo.Contains("(") ? clientInfo.Substring(0, clientInfo.IndexOf("(")).Trim() : clientInfo;
            string clientIp = clientInfo.Contains("(") ? clientInfo.Substring(clientInfo.IndexOf("(") + 1, clientInfo.IndexOf(")") - clientInfo.IndexOf("(") - 1) : "";

            AddClient(playerName, clientIp);
        }

        private void TcpServer_OnClientDisconnected(string clientInfo)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => TcpServer_OnClientDisconnected(clientInfo)));
                return;
            }

            RemoveClient(clientInfo);
        }

        private void TcpServer_OnLogMessage(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => TcpServer_OnLogMessage(message)));
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[TCP Server] {message}");
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
