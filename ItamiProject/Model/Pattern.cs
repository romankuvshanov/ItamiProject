using System;
using System.Numerics;

namespace Model
{
    public class Pattern
    {
        public readonly Projectile[] Projectiles;

        public Pattern(int projectileNumber, int hitbox, Vector2 location)
        {
            Projectiles = new Projectile[projectileNumber];
            for (int i = 0; i < projectileNumber; i++) Projectiles[i] = new Projectile(hitbox);
            SetToStartingPoint(location);
        }

        public void SetToStartingPoint(Vector2 location)
        {
            for (int i = 0; i < Projectiles.Length; i++)
            {
                Projectiles[i].Location = location;
            }
        }

        public void MoveProjectiles()
        {
            double step = 360 / Projectiles.Length;
            double angle = 0;
            float speed = 5;
            for (int i = 0; i < Projectiles.Length; i++)
            {
                Projectiles[i].Location.X += (float)(speed * Math.Cos(angle));
                Projectiles[i].Location.Y += (float)(speed * Math.Sin(angle));
                angle += step;
            }
        }
    }
}