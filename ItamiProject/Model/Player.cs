using System.Collections.Generic;
using System.Numerics;

namespace Model
{
    public class Player : Character
    {
        public int ShiftModifier;
        public int Lives;

        public Player(Vector2 location, float width, float height, int speed, int shiftModifier)
            : base(location, width, height, speed)
        {
            ShiftModifier = shiftModifier;
        }

        public void SetPlayerLivesNumber(int amount)
        {
            Lives = amount;
        }

        public void Fire(List<Projectile> PlayerProjectiles)
        {
            var proj = new Projectile(20);
            proj.Location = Location;
            PlayerProjectiles.Add(proj);
        }
    }
}