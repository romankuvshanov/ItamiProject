using GameModel;
using System.Collections.Generic;
using System.Windows.Forms;
namespace GameController
{
    public class Controller
    {
        Game game;
        HashSet<Keys> keySet;

        public Controller(Game game)
        {
            this.game = game;
            keySet = new HashSet<Keys>();
        }

        public void SetPlayerLivesNumber(int amount)
        {
            game.player.SetPlayerLivesNumber(amount);
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
            if (keySet.Contains(Keys.W) && key == Keys.S) keySet.Remove(Keys.W);
            else if (keySet.Contains(Keys.S) && key == Keys.W) keySet.Remove(Keys.S);
            if (keySet.Contains(Keys.A) && key == Keys.D) keySet.Remove(Keys.A);
            else if (keySet.Contains(Keys.D) && key == Keys.A) keySet.Remove(Keys.D);
            keySet.Add(key);
        }

        public void RemoveKeyFromSet(Keys key)
        {
            keySet.Remove(key);
        }

        public void IterateGameCycle()
        {
            game.MovePlayer(keySet);
            game.MoveEnemy();
            game.MoveProjectiles();
            game.CheckForCollision();
        }
    }
}