using System.Collections.Generic;
using System.Numerics;

namespace Model
{
    public class Player : Character
    {
        public int ShiftModifier;
        public int ExtraLives;
        public bool IsDead { get; private set; }

        public Player(Vector2 location, float width, float height, int speed, int shiftModifier)
            : base(location, width, height, speed)
        {
            ShiftModifier = shiftModifier;
        }

        public void Attack(List<Projectile> PlayerProjectiles)
        {
            var p = new Projectile(20);
            p.Location = Location;
            PlayerProjectiles.Add(p);
        }

        public void Die()
        {
            IsDead = true;
        }
    }
}