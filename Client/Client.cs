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

            // Show player name input dialog first
            PlayerNameForm playerNameForm = new PlayerNameForm();
            if (playerNameForm.ShowDialog() == DialogResult.OK)
            {
                // If player entered name, show the main client form
                Application.Run(new ClientForm(playerNameForm.PlayerName));
            }
            // If cancelled, exit application
        }
    }
}
