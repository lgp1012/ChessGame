using System;
using System.Windows.Forms;

namespace Client
{
    public partial class ServerConfigForm : Form
    {
        public string ServerIP { get; private set; }
        public int ServerPort { get; private set; }

        public ServerConfigForm(string currentIP = "192.168.100.2", int currentPort = 5000)
        {
            InitializeComponent();
            txtServerIP.Text = currentIP;
            txtServerPort.Text = currentPort.ToString();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerIP.Text))
            {
                MessageBox.Show("Please enter server IP address", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtServerPort.Text, out int port) || port <= 0 || port > 65535)
            {
                MessageBox.Show("Please enter a valid port number (1-65535)", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServerIP = txtServerIP.Text.Trim();
            ServerPort = port;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
