using System.Collections.Generic;
using System.Windows.Forms;
using System;
using System.Numerics;

namespace Model
{
    public class Game
    {
        // Параметры
        public int Width = 1280;
        public int Height = 720;
        public Difficulty DifficultyLevel;
        public byte ScoreMultiplier;

        // Состояния
        public bool ShiftIsDown;
        private DateTime _collisionTime;

        // Сущности
        public Player Player;
        public Enemy Enemy;
        public Pattern Pattern;
        public List<Projectile> PlayerProjectiles;

        public Game()
        {
            Player = new Player(new Vector2(Width / 2, Height * 5 / 6), 29, 54, 5, -2);
            Enemy = new Enemy(new Vector2(Width / 2, Height / 6), 29, 54, 4, 1000);
            Pattern = new Pattern(50, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
            PlayerProjectiles = new List<Projectile>();
        }

        public void Start()
        {
            _collisionTime = DateTime.Now;
            if (DifficultyLevel == Difficulty.Easy)
            {
                Enemy.HP = 500;
                Pattern = new Pattern(25, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                ScoreMultiplier = 1;
            }
            else if (DifficultyLevel == Difficulty.Medium)
            {
                Enemy.HP = 1000;
                Pattern = new Pattern(50, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                ScoreMultiplier = 2;
            }
            else if (DifficultyLevel == Difficulty.Hard)
            {
                Enemy.HP = 5000;
                Pattern = new Pattern(250, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                ScoreMultiplier = 10;
            }
        }

        public void MoveFires()
        {
            for (int i = 0; i < PlayerProjectiles.Count; i++)
            {
                PlayerProjectiles[i].Location.Y -= 7;
                if (PlayerProjectiles[i].Location.Y < 0) PlayerProjectiles.RemoveAt(i);
            }
        }

        public int CheckForFireCollision()
        {
            int removed = PlayerProjectiles.RemoveAll(p => Math.Abs(p.Location.X - Enemy.Location.X) < p.Hitbox&& Math.Abs(p.Location.Y - Enemy.Location.Y) < p.Hitbox);
            Enemy.HP -= 10 * removed;
            //for (int i = 0; i < PlayerProjectiles.Count; i++)
            //{
            //    if (Math.Abs(PlayerProjectiles[i].Location.X - Enemy.Location.X) < PlayerProjectiles[i].Hitbox
            //        && Math.Abs(PlayerProjectiles[i].Location.Y - Enemy.Location.Y) < PlayerProjectiles[i].Hitbox)
            //    {
            //        /* 
            //         * NOTE: В данмаку у врага не предполагаются 'invincibility frames' после получения урона,
            //         * только у игрока. У врага ОЧЕНЬ много хп, и игрок должен (стараться) попадать по нему всё время.
            //        */
            //        Enemy.HP -= 10;
            //        PlayerProjectiles.RemoveAt(i);
            //        wasHit = true;
            //    }
            //}
            return removed;
        }

        public bool CheckForCollision()
        {
            foreach (var projectile in Pattern.Projectiles)
            {
                if (projectile.Location.X < Player.Location.X + Player.Width
                    && projectile.Location.X + projectile.Hitbox > Player.Location.X
                    && projectile.Location.Y < Player.Location.Y + Player.Height
                    && projectile.Location.Y + projectile.Hitbox > Player.Location.Y)
                {
                    // Если после попадания не прошло 3 секунды, то повторного попадания нет
                    if ((DateTime.Now - _collisionTime).TotalMilliseconds > 3000)
                    {
                        _collisionTime = DateTime.Now;
                        Player.Lives--;
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveProjectiles()
        {
            foreach (var projectile in Pattern.Projectiles)
            {
                if (projectile.Location.X + projectile.Hitbox * 2 > 0
                    && projectile.Location.X < Width
                    && projectile.Location.Y + projectile.Hitbox * 2 > 0
                    && projectile.Location.Y < Height)
                {
                    Pattern.MoveProjectiles();
                    return;
                }
            }
            Pattern.SetToStartingPoint(Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
        }

        public void MoveEnemy()
        {
            if (Enemy.Location.X > Width * 3 / 4 || Enemy.Location.X + Enemy.Width < Width / 4) Enemy.Speed = -Enemy.Speed;
            Enemy.Location.X += Enemy.Speed;
        }

        public void MovePlayer(HashSet<Keys> keySet)
        {
            int speed = ShiftIsDown ? Player.Speed + Player.ShiftModifier : Player.Speed;
            if (keySet.Contains(Keys.W) && Player.Location.Y > 0) Player.Location.Y -= speed;
            if (keySet.Contains(Keys.A) && Player.Location.X > 0) Player.Location.X -= speed;
            if (keySet.Contains(Keys.S) && Player.Location.Y + Player.Height < Height) Player.Location.Y += speed;
            if (keySet.Contains(Keys.D) && Player.Location.X + Player.Width < Width) Player.Location.X += speed;
        }
    }
}