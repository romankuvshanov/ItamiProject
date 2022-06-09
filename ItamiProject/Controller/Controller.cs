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
                if (_actionSet.Contains(PlayerAction.MoveU) && action == PlayerAction.MoveD) _actionSet.Remove(PlayerAction.MoveU);
                else if (_actionSet.Contains(PlayerAction.MoveD) && action == PlayerAction.MoveU) _actionSet.Remove(PlayerAction.MoveD);
                if (_actionSet.Contains(PlayerAction.MoveL) && action == PlayerAction.MoveR) _actionSet.Remove(PlayerAction.MoveL);
                else if (_actionSet.Contains(PlayerAction.MoveR) && action == PlayerAction.MoveL) _actionSet.Remove(PlayerAction.MoveR);
                _actionSet.Add(action);
            }
            else _actionSet.Remove(action);
        }

        public void IterateGameCycle(int elapsedTime)
        {
            _game.SetPlayerToAction(_actionSet, elapsedTime);
            _game.MoveEnemy();
            _game.MoveProjectiles();
            _game.MoveFires();
        }
    }
}