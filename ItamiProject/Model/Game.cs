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

        // Состояния
        public bool ShiftIsDown;
        public DateTime _collisionTime;

        // Сущности
        public Player Player;
        public Enemy Enemy;
        public Pattern Pattern;

        public Game()
        {
            Player = new Player(new Vector2(Width / 2, Height * 5 / 6), 29, 54, 5, -2);
            Enemy = new Enemy(new Vector2(Width / 2, Height / 6), 29, 54, 4, 1000);
            Pattern = new Pattern(100, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
        }

        public void StartGame()
        {
            _collisionTime = DateTime.Now;
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
                        Player.Lifes--;
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