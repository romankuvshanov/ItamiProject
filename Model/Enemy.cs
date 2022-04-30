namespace GameModel
{
    public class Enemy
    {
        // Пока не используется
        private int HP;

        public int x;
        public int y;
        public int speed;
        public Enemy(int x, int y)
        {
            this.x = x;
            this.y = y;
            speed = 4;
        }
    }
}
