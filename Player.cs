using System.Collections.Generic;
using System.Drawing;

namespace ItamiProject
{
    internal class Player
    {
        public int x, y;

        public int health;

        public List<Point> boombsCoords = new List<Point>();

        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
            this.health = 100;
        }
    }
}