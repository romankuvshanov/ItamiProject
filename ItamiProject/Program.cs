using System;
using View;
using System.Windows.Forms;

namespace ItamiProject
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameWindow());
        }
    }
}
