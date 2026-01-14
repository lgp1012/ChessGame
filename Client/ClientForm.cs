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

        // Constructor bắt buộc nhận playerName và serverIP
        public ClientForm(string name, string ip)
        {
            InitializeComponent();
            playerName = name;
            serverIP = ip;
            
            // Update title with player name
            if (!string.IsNullOrEmpty(playerName))
            {
                this.Text = $"Chess Game - {playerName}";
                lblTitle.Text = $"Chess Game Client - {playerName}";
            }

            // Initialize TCP connection với server IP
            tcpConnection = new TcpClientConnection(serverIP);
            tcpConnection.OnMessageReceived += TcpConnection_OnMessageReceived;
            tcpConnection.OnConnectionStatusChanged += TcpConnection_OnConnectionStatusChanged;
            tcpConnection.OnError += TcpConnection_OnError;

            // Initialize button states
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
        }

        private void TcpConnection_OnMessageReceived(string message)
        {
            UpdateUI($"[Server] {message}");
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
            tcpConnection.Disconnect();
        }

        private void UpdateUI(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUI(message)));
                return;
            }

            // Update status label text and color
            bool connected = tcpConnection != null && tcpConnection.IsConnected;
            lblStatus.Text = $"Status: {(connected ? "Connected" : "Disconnected")}";
            lblStatus.ForeColor = connected ? Color.Green : Color.Red;

            // Enable/disable Connect/Disconnect buttons based on connection state
            btnConnect.Enabled = !connected;
            btnDisconnect.Enabled = connected;

            // Append log/message
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
