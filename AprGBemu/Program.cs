using System;
using System.Windows.Forms;
using System.IO;

namespace AprGBemu
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            if (!File.Exists(Application.StartupPath + "/AprGBemuLang.ini"))
            {
                MessageBox.Show("Missing AprGBemuLang.ini language file , exit..");
                return;
            }

            AppDomain.CurrentDomain.AppendPrivatePath(Application.StartupPath + "/DLLs");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(AprGBemu_MainUI.GetInstance());
        }
    }
}
