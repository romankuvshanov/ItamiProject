using Model;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Controller
{
    public class GameController
    {
        readonly Game _game;
        readonly HashSet<Keys> _keySet = new HashSet<Keys>();

        public GameController(Game game)
        {
            _game = game;
        }

        public bool HasCollisionOccured()
        {
            return _game.CheckForCollision();
        }

        public bool HasFireCollisionOccured()
        {
            return _game.CheckForFireCollision();
        }

        public void StartGame()
        {
            _game.StartGame();
        }

        public void SetPlayerLivesNumber(int amount)
        {
            _game.Player.SetPlayerLivesNumber(amount);
        }

        public void AddKeyToSet(Keys key)
        {
            /*
             * Это, чтобы работало так: нажал A с зажатым D -> персонаж, который
             * движется вправо, сразу начинает двигаться влево
             * 
             * Но в таком виде это работает так: персонаж, который
             * движется вправо, сразу начинает двигаться влево, НО!!!
             * при отпускании A (со всё ещё зажатым D) движение обратно
             * вправо не начинается
             * 
             * TODO: пофиксить это :) ИЛИ придумать решение получше
             * 
             * UPDATE: в тохе проблема такая же, но только в одном направлении
             */
            if (_keySet.Contains(Keys.W) && key == Keys.S) _keySet.Remove(Keys.W);
            else if (_keySet.Contains(Keys.S) && key == Keys.W) _keySet.Remove(Keys.S);
            if (_keySet.Contains(Keys.A) && key == Keys.D) _keySet.Remove(Keys.A);
            else if (_keySet.Contains(Keys.D) && key == Keys.A) _keySet.Remove(Keys.D);
            _keySet.Add(key);
        }

        public void RemoveKeyFromSet(Keys key)
        {
            _keySet.Remove(key);
        }

        public void HandleShift(bool pressed)
        {
            if (pressed) _game.ShiftIsDown = true;
            else _game.ShiftIsDown = false;
        }

        public void IterateGameCycle()
        {
            _game.MovePlayer(_keySet);
            _game.MoveEnemy();
            _game.MoveProjectiles();
            _game.MoveFires();
        }
    }
}