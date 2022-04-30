using GameView;
using GameModel;
using GameController;
namespace Main
{
    internal class MainClass
    {
        static void Main()
        {
            Game game = new Game();
            Controller ctrl = new Controller(game);
            GameWindow wnd = new GameWindow(game, ctrl);
            wnd.ShowDialog();
        }
    }
}