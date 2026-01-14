using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    internal static class Client
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show player name and server IP input dialog first
            PlayerNameForm playerNameForm = new PlayerNameForm();
            if (playerNameForm.ShowDialog() == DialogResult.OK)
            {
                // If player entered name and server IP, show the main client form
                Application.Run(new ClientForm(playerNameForm.PlayerName, playerNameForm.ServerIP));
            }
            // If cancelled, exit application
        }
    }
}
