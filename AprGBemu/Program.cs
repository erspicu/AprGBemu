using System;
using System.Windows.Forms;

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
            
            AppDomain.CurrentDomain.AppendPrivatePath(Application.StartupPath + "\\DLLs");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(AprGBemu_MainUI.GetInstance());
        }
    }
}
