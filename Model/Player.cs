namespace GameModel
{
    public class Player : Character
    {
        public int ShiftModifier;
        public int Lifes;

        public Player(int x, int y, int speed, int shiftModifier)
            : base(x, y, speed)
        {
            ShiftModifier = shiftModifier;
        }

        public void SetPlayerLivesNumber(int amount)
        {
            Lifes = amount;
        }
    }
}