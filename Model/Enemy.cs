namespace GameModel
{
    public class Enemy : Character
    {
        // Пока не используется
        private int HP;

        public Enemy(int x, int y, int speed, int HP)
            : base(x, y, speed)
        {
            this.HP = HP;
        }
    }
}