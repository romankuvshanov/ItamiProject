namespace GameModel
{
    public class Player : Character
    {
        public int shiftModifier;
        public int lifes;

        public Player(int x, int y, int speed, int shiftModifier)
            : base(x, y, speed)
        {
            this.shiftModifier = shiftModifier;
        }

        public void SetPlayerLivesNumber(int amount)
        {
            lifes = amount;
        }
    }
}