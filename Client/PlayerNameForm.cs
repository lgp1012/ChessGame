using System;
using System.Windows.Forms;

namespace Client
{
    public partial class PlayerNameForm : Form
    {
        public string PlayerName { get; private set; }
        public string ServerIP { get; private set; }

        public PlayerNameForm()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPlayerName.Text))
            {
                MessageBox.Show("Please enter your player name", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtServerIP.Text))
            {
                MessageBox.Show("Please enter server IP address", "Input Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PlayerName = txtPlayerName.Text.Trim();
            ServerIP = txtServerIP.Text.Trim();
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
