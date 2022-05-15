using System;
namespace GameModel
{
    public class Pattern
    {
        public readonly Projectile[] Projectiles;

        public Pattern(int projectileNumber, int hitbox, int xPosition, int yPosition)
        {
            Projectiles = new Projectile[projectileNumber];
            for (int i = 0; i < projectileNumber; i++) Projectiles[i] = new Projectile(hitbox);
            SetToStartingPoint(xPosition, yPosition);
        }

        public void SetToStartingPoint(int xPosition, int yPosition)
        {
            for (int i = 0; i < Projectiles.Length; i++)
            {
                Projectiles[i].X = xPosition;
                Projectiles[i].Y = yPosition;
            }
        }

        /*
         * NOTE: Возможно, пора перейти на float для координат
         * игрока, снарядов, врагов и т. д., т. к. целочисленные координаты
         * частично ограничивают "креативные" возможности.
         * 
         * Предлагаю обсудить это :)
         */
        public void MoveProjectiles()
        {
            double step = 360 / Projectiles.Length;
            double angle = 0;
            int xSpeed = 5;
            int ySpeed = 5;
            for (int i = 0; i < Projectiles.Length; i++)
            {
                Projectiles[i].X += (int)(xSpeed * Math.Cos(angle));
                Projectiles[i].Y += (int)(ySpeed * Math.Sin(angle));
                angle += step;
            }
        }
    }
}