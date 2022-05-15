using System.Collections.Generic;
using System.Windows.Forms;
using System;
namespace GameModel
{
    public class Game
    {
        // Параметры
        public int Width = 1280;
        public int Height = 720;

        // Состояния
        public bool ShiftIsDown;
        public DateTime CollisionTime = DateTime.Now;

        // Сущности
        public Player Player;
        public Enemy Enemy;
        public Pattern Pattern;

        public Game()
        {
            Player = new Player(Width / 2, Height * 3 / 4, 5, -2);
            Enemy = new Enemy(Width / 2, Height / 4, 4, 1000);
            Pattern = new Pattern(15, 10, Enemy.X + 14, Enemy.Y + 54);
        }

        public void CheckForCollision()
        {
            foreach (var projectile in Pattern.Projectiles)
            {
                if (projectile.X < Player.X + 29
                    && projectile.X + projectile.Hitbox > Player.X
                    && projectile.Y < Player.Y + 54
                    && projectile.Y + projectile.Hitbox > Player.Y)
                {
                    // Если после попадания не прошло 3 секунды, то повторного попадания нет
                    if ((DateTime.Now - CollisionTime).TotalMilliseconds > 3000)
                    {
                        CollisionTime = DateTime.Now;
                        Player.Lifes--;
                    }
                    break;
                }
            }
        }

        public void MoveProjectiles()
        {
            foreach (var projectile in Pattern.Projectiles)
            {
                if (projectile.X + projectile.Hitbox * 2 > 0
                    && projectile.X < Width
                    && projectile.Y + projectile.Hitbox * 2 > 0
                    && projectile.Y < Height)
                {
                    Pattern.MoveProjectiles();
                    return;
                }
            }
            Pattern.SetToStartingPoint(Enemy.X + 14, Enemy.Y + 54);
        }

        public void MoveEnemy()
        {
            if (Enemy.X > Width * 3 / 4 || Enemy.X < Width / 4) Enemy.Speed = -Enemy.Speed;
            Enemy.X += Enemy.Speed;
        }
        
        public void MovePlayer(HashSet<Keys> keySet)
        {
            if (ShiftIsDown)
            {
                if (keySet.Contains(Keys.W) && Player.Y > 0) Player.Y -= Player.Speed + Player.ShiftModifier;
                if (keySet.Contains(Keys.A) && Player.X > 0) Player.X -= Player.Speed + Player.ShiftModifier;
                if (keySet.Contains(Keys.S) && Player.Y < Height - 54) Player.Y += Player.Speed + Player.ShiftModifier;
                if (keySet.Contains(Keys.D) && Player.X < Width - 29) Player.X += Player.Speed + Player.ShiftModifier;
            }
            else
            {
                if (keySet.Contains(Keys.W) && Player.Y > 0) Player.Y -= Player.Speed;
                if (keySet.Contains(Keys.A) && Player.X > 0) Player.X -= Player.Speed;
                if (keySet.Contains(Keys.S) && Player.Y < Height - 54) Player.Y += Player.Speed;
                if (keySet.Contains(Keys.D) && Player.X < Width - 29) Player.X += Player.Speed;
            }
        }
    }
}