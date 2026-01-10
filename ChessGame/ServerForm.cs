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

        public ServerForm()
        {
            InitializeComponent();
            UpdateUI();
        }

        void AddClient(string clientInfo)
        {
            clientList.Items.Add(clientInfo);
            connectedClients++;
            UpdateUI();
        }

        void RemoveClient(string clientInfo)
        {
            if (clientList.Items.Contains(clientInfo))
            {
                clientList.Items.Remove(clientInfo);
                connectedClients--;
                UpdateUI();
            }
        }

        private void UpdateUI()
        {
            lblServerStatus.Text = serverRunning
            ? "Server status: RUNNING"
            : "Server status: STOPPED";

            lblConnectClient.Text = $"Connected clients: {connectedClients} / 2";

            btnStartMatch.Enabled = serverRunning && connectedClients == 2 && (countdownTimer == null || !countdownTimer.Enabled);

            btnStopMatch.Enabled = matchStarted;
        }


        private void btnStopMatch_Click(object sender, EventArgs e)
        {
            //Trận đấu chưa bắt đầu thì nút stop match này vẫn bị vô hiệu hóa
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
            clientList.Items.Clear();
            lblMatchStatus.Text = "Waiting for players...";

            // Start the TCP server to accept client connections
            tcpServer = new TcpServer();
            tcpServer.OnClientConnected += TcpServer_OnClientConnected;
            tcpServer.OnClientDisconnected += TcpServer_OnClientDisconnected;
            tcpServer.OnLogMessage += TcpServer_OnLogMessage;
            tcpServer.Start();

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
            clientList.Items.Clear();

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

            AddClient(clientInfo);
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
    }
}
