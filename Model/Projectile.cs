using System.Numerics;
namespace GameModel
{
    public class Projectile
    {
        /*
         * Радиус хитбокса.
         * 
         * Возможно, стоит переименовать, либо создать класс специально
         * для хитбокса, т. к. слово hitbox всё-таки подразумевает... box.
         */
        public int Hitbox;

        public Vector2 Location;

        public Projectile(int hitbox)
        {
            Hitbox = hitbox;
        }
    }
}