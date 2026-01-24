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

            // Mở form để nhập tên người chơi và địa chỉ IP máy chủ
            PlayerNameForm playerNameForm = new PlayerNameForm();
            if (playerNameForm.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new ClientForm(playerNameForm.PlayerName, playerNameForm.ServerIP));
            }
        }
    }
}
