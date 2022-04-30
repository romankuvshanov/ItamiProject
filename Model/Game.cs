using System.Collections.Generic;
using System.Windows.Forms;
namespace GameModel
{
    public class Game
    {
        public int Width;
        public int Height;
        public Player player;
        public bool shiftIsDown;
        public Game()
        {
            Width = 1280;
            Height = 720;
            player = new Player(Width / 2, Height * 3 / 4);
        }

        public void MovePlayer(HashSet<Keys> keySet)
        {
            if (keySet.Contains(Keys.ShiftKey))
            {
                shiftIsDown = true;
                if (keySet.Contains(Keys.W) && player.y > 0) player.y -= player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.A) && player.x > 0) player.x -= player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.S) && player.y < Height - 54) player.y += player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.D) && player.x < Width - 29) player.x += player.speed + player.shiftModifier;
            }
            else
            {
                shiftIsDown = false;
                if (keySet.Contains(Keys.W) && player.y > 0) player.y -= player.speed;
                if (keySet.Contains(Keys.A) && player.x > 0) player.x -= player.speed;
                if (keySet.Contains(Keys.S) && player.y < Height - 54) player.y += player.speed;
                if (keySet.Contains(Keys.D) && player.x < Width - 29) player.x += player.speed;
            }
        }
    }
}