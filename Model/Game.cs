using System.Collections.Generic;
using System.Windows.Forms;
using System;
namespace GameModel
{
    public class Game
    {
        // Параметры
        public int Width;
        public int Height;

        // Состояния
        public bool shiftIsDown;
        public DateTime collisionTime = DateTime.Now;

        // Сущности
        public Player player;
        public Enemy enemy;
        public Projectile[] projectiles;

        public Game()
        {
            Width = 1280;
            Height = 720;
            player = new Player(Width / 2, Height * 3 / 4, 5, -2);
            enemy = new Enemy(Width / 2, Height / 4, 4, 1000);
            projectiles = new Projectile[15];
            projectiles[0] = new Projectile(15, enemy.x - 58, enemy.y + 54);
            projectiles[5] = new Projectile(15, enemy.x, enemy.y + 108);
            projectiles[10] = new Projectile(15, enemy.x + 58, enemy.y + 54);
            for (int i = 1; i <= 4; i++)
            {
                projectiles[i] = new Projectile(15, projectiles[i - 1].x - 15, projectiles[i - 1].y + 20);
            }
            for (int i = 6; i <= 9; i++)
            {
                projectiles[i] = new Projectile(15, projectiles[i - 1].x, projectiles[i - 1].y + 25);
            }
            for (int i = 11; i <= 14; i++)
            {
                projectiles[i] = new Projectile(15, projectiles[i - 1].x + 15, projectiles[i - 1].y + 20);
            }
        }

        public void CheckForCollision()
        {
            foreach (var projectile in projectiles)
            {
                if (projectile.x < player.x + 29
                    && projectile.x + projectile.hitbox > player.x
                    && projectile.y < player.y + 54
                    && projectile.y + projectile.hitbox > player.y)
                {
                    // Если после попадания не прошло 3 секунды, то повторного попадания нет
                    if ((DateTime.Now - collisionTime).TotalMilliseconds > 3000)
                    {
                        collisionTime = DateTime.Now;
                        player.lifes--;
                    }
                    break;
                }
            }
        }

        public void MoveProjectiles()
        {
            if (projectiles[0].y >= Height)
            {
                projectiles[0].x = enemy.x - 58;
                projectiles[0].y = enemy.y + 54;

                projectiles[5].x = enemy.x;
                projectiles[5].y = enemy.y + 108;

                projectiles[10].x = enemy.x + 58;
                projectiles[10].y = enemy.y + 54;

                for (int i = 1; i <= 4; i++)
                {
                    projectiles[i].x = projectiles[i - 1].x - 15;
                    projectiles[i].y = projectiles[i - 1].y + 20;
                }
                for (int i = 6; i <= 9; i++)
                {
                    projectiles[i].x = projectiles[i - 1].x;
                    projectiles[i].y = projectiles[i - 1].y + 25;
                }
                for (int i = 11; i <= 14; i++)
                {
                    projectiles[i].x = projectiles[i - 1].x + 15;
                    projectiles[i].y = projectiles[i - 1].y + 20;
                }
            }
            else
            {
                for (int i = 0; i <= 4; i++)
                {
                    projectiles[i].x -= 5;
                    projectiles[i].y += 5;
                }
                for (int i = 5; i <= 9; i++)
                {
                    projectiles[i].y += 5;
                }
                for (int i = 10; i <= 14; i++)
                {
                    projectiles[i].x += 5;
                    projectiles[i].y += 5;
                }
            }
        }

        public void MoveEnemy()
        {
            if (enemy.x > Width * 3 / 4 || enemy.x < Width / 4) enemy.speed = -enemy.speed;
            enemy.x += enemy.speed;
        }
        
        public void MovePlayer(HashSet<Keys> keySet)
        {
            if (keySet.Contains(Keys.ShiftKey))
            {
                shiftIsDown = true;
                if (keySet.Contains(Keys.W) && player.y > 0) player.y -= player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.A) && player.x > 0) player.x -= player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.S) && player.y < Height - 54) player.y += player.speed + player.shiftModifier;
                if (keySet.Contains(Keys.D) && player.x < Width - 29) player.x += player.speed + player.shiftModifier;
            }
            else
            {
                shiftIsDown = false;
                if (keySet.Contains(Keys.W) && player.y > 0) player.y -= player.speed;
                if (keySet.Contains(Keys.A) && player.x > 0) player.x -= player.speed;
                if (keySet.Contains(Keys.S) && player.y < Height - 54) player.y += player.speed;
                if (keySet.Contains(Keys.D) && player.x < Width - 29) player.x += player.speed;
            }
        }
    }
}