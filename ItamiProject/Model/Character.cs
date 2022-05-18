using System.Numerics;

namespace Model
{
    public class Character
    {
        public Vector2 Location;
        public float Width;
        public float Height;
        public int Speed;

        public Character(Vector2 location, float width, float height, int speed)
        {
            Location = location;
            Width = width;
            Height = height;
            Speed = speed;
        }
    }
}
