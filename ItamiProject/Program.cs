using System;
using Model;
using View;
using Controller;
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
            Game game = new Game();
            GameController ctrl = new GameController(game);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GameWindow(game, ctrl));
        }
    }
}
