using Model;
using System.Collections.Generic;

namespace Controller
{
    public class GameController
    {
        private readonly Game _game;
        private readonly HashSet<PlayerAction> _actionSet = new HashSet<PlayerAction>();

        public GameController(Game game)
        {
            _game = game;
        }

        public void StartGame(int livesAmount, int difficultyIndex)
        {
            _game.Player.ExtraLives = livesAmount;
            _game.DifficultyLevel = (Difficulty)difficultyIndex;
            _game.Start();
        }

        public bool WasEnemyHit()
        {
            return _game.CheckFireCollision() != 0;
        }

        public bool WasPlayerHit()
        {
            return _game.CheckCollision();
        }

        public void HandleShift(bool isDown)
        {
            if (isDown) _game.ShiftIsDown = true;
            else _game.ShiftIsDown = false;
        }

        public void HandleInput(PlayerAction action, bool isDown)
        {
            if (isDown)
            {
                if (_actionSet.Contains(PlayerAction.MoveUp) && action == PlayerAction.MoveDown) _actionSet.Remove(PlayerAction.MoveUp);
                else if (_actionSet.Contains(PlayerAction.MoveDown) && action == PlayerAction.MoveUp) _actionSet.Remove(PlayerAction.MoveDown);
                if (_actionSet.Contains(PlayerAction.MoveLeft) && action == PlayerAction.MoveRight) _actionSet.Remove(PlayerAction.MoveLeft);
                else if (_actionSet.Contains(PlayerAction.MoveRight) && action == PlayerAction.MoveLeft) _actionSet.Remove(PlayerAction.MoveRight);
                _actionSet.Add(action);
            }
            else _actionSet.Remove(action);
        }

        public void IterateGameCycle(int elapsedTime)
        {
            _game.SetPlayerToAction(_actionSet, elapsedTime);
            _game.MoveEnemy();
            _game.MoveProjectiles(elapsedTime);
            _game.MoveFires();
        }
    }
}