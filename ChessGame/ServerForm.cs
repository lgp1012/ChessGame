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

        private void UpdateUI()
        {
            lblServerStatus.Text = serverRunning
            ? "Server status: RUNNING"
            : "Server status: STOPPED";

            lblConnectClient.Text = $"Connected clients: {connectedClients} / 2";

            btnStartMatch.Enabled = serverRunning && connectedClients == 2 && (countdownTimer == null || !countdownTimer.Enabled);
        }


        private void btnStopMatch_Click(object sender, EventArgs e)
        {
            // Nếu bộ đếm đang trong giai đoạn đếm thì sẽ dừng lập tức
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            lblMatchStatus.Text = "Match paused by server";
            UpdateUI();
        }

        private void btnStartServer_Click(object sender, EventArgs e)
        {
            serverRunning = true;
            lblMatchStatus.Text = "Waiting for players...";
            AddClient("1");
            AddClient("2");
            UpdateUI();
        }

        private void btnEndServer_Click(object sender, EventArgs e)
        {
            // Nếu bộ đếm đang trong giai đoạn đếm thì sẽ dừng lập tức
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            serverRunning = false;
            connectedClients = 0;
            clientList.Items.Clear();

            lblMatchStatus.Text = "Server stopped";
            UpdateUI();
        }

        private void StartCountdown(int seconds)
        {
            // Đảm bảo rằng không còn Timer trước đó được lưu trữ lại
            if (countdownTimer != null)
            {
                countdownTimer.Stop();
                countdownTimer.Dispose();
                countdownTimer = null;
            }

            countdownSeconds = seconds;
            lblMatchStatus.Text = $"Match starts in {countdownSeconds} seconds...";

            countdownTimer = new System.Windows.Forms.Timer();
            countdownTimer.Interval = 1000; // 1 second
            countdownTimer.Tick += (s, ev) =>
            {
                countdownSeconds--;
                if (countdownSeconds > 0)
                {
                    lblMatchStatus.Text = $"Match starts in {countdownSeconds} seconds...";
                }
                else
                {
                    // Quá trình đếm ngược hoàn tất thì xóa bộ đếm Timer
                    countdownTimer.Stop();
                    countdownTimer.Dispose();
                    countdownTimer = null;

                    lblMatchStatus.Text = "Match started!";
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

        
    }
}
