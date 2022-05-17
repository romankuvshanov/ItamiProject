using System.Numerics;
namespace GameModel
{
    public class Player : Character
    {
        public int ShiftModifier;
        public int Lifes;

        public Player(Vector2 location, float width, float height, int speed, int shiftModifier)
            : base(location, width, height, speed)
        {
            ShiftModifier = shiftModifier;
        }

        public void SetPlayerLivesNumber(int amount)
        {
            Lifes = amount;
        }
    }
}