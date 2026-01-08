using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGame
{
    public partial class ServerForm : Form
    {
        private bool serverRunning = false;
        private int connectedClients = 0;
        public ServerForm()
        {
            InitializeComponent();
            UpdateUI();
        }

        private void UpdateUI()
        {
            lblServerStatus.Text = serverRunning
            ? "Server status: RUNNING"
            : "Server status: STOPPED";

            lblConnectClient.Text = $"Connected clients: {connectedClients} / 2";
        }

        private void lblServerStatus_Click(object sender, EventArgs e)
        {

        }

        private void clientList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void NameGame_Click(object sender, EventArgs e)
        {

        }

        private void lblMatchStatus_Click(object sender, EventArgs e)
        {

        }

        private void btnStopMatch_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
