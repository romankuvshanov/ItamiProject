using System.Numerics;
namespace GameModel
{
    public class Enemy : Character
    {
        // Пока не используется
        private int HP;

        public Enemy(Vector2 location, float width, float height, int speed, int HP)
            : base(location, width, height, speed)
        {
            this.HP = HP;
        }
    }
}