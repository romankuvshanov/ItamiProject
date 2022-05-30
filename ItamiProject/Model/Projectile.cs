using System.Numerics;

namespace Model
{
    public class Projectile
    {
        /*
         * Радиус хитбокса.
         * 
         * Возможно, стоит переименовать, либо создать класс специально
         * для хитбокса, т. к. слово hitbox всё-таки подразумевает... box.
         * 
         * NOTE: пока переименовал в диаметр
         */
        public int Diameter;

        public Vector2 Location;

        public Projectile(int diameter)
        {
            Diameter = diameter;
        }
    }
}