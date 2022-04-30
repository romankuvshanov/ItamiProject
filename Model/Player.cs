namespace GameModel
{
    public class Player
    {
        public int x;
        public int y;
        public int speed;
        public int shiftModifier;
        public Player(int x, int y)
        {
            this.x = x;
            this.y = y;
            speed = 5;
            shiftModifier = -2;
        }
    }
}