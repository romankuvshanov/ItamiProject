using System.Collections.Generic;
using System.Windows.Forms;
namespace GameModel
{
    public class Game
    {
        public int Width;
        public int Height;
        public Player player;
        public Enemy enemy;
        public bool shiftIsDown;
        public Game()
        {
            Width = 1280;
            Height = 720;
            player = new Player(Width / 2, Height * 3 / 4);
            enemy = new Enemy(Width / 2, Height / 4);
        }

        public void MoveEnemy()
        {
            if (enemy.x > Width * 3 / 4 || enemy.x < Width / 4) enemy.speed = -enemy.speed;
            enemy.x += enemy.speed;
        }

        public void MovePlayer(HashSet<Keys> keySet)
        {
            if (keySet.Contains(Keys.ShiftKey))
            {
                shiftIsDown = true;
                if (keySet.Contains(Keys.W) && player.y > 0) player.y -= player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.A) && player.x > 0) player.x -= player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.S) && player.y < Height) player.y += player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.D) && player.x < Width) player.x += player.speed + player.shiftModifier;
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