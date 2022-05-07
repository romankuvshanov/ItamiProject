namespace GameModel
{
    public class Projectile
    {
        public int hitbox; // радиус хитбокса
        public int x;
        public int y;

        public Projectile(int hitbox, int x, int y)
        {
            this.hitbox = hitbox;
            this.x = x;
            this.y = y;
        }
    }
}