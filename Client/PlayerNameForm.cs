using System;
using System.Windows.Forms;

namespace Client
{
    public partial class PlayerNameForm : Form
    {
        public string PlayerName { get; private set; }

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

            PlayerName = txtPlayerName.Text.Trim();
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
