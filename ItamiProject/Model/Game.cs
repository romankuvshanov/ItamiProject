using System.Collections.Generic;
using System;
using System.Numerics;

namespace Model
{
    public class Game
    {
        // Параметры
        private int _width = 768;
        private int _height = 576;
        public Difficulty DifficultyLevel;
        public byte ScoreMultiplier { get; private set; }

        // Состояния
        public bool ShiftIsDown;
        private DateTime _collisionTime;

        // Сущности
        public Player Player { get; private set; }
        public Enemy Enemy { get; private set; }
        public Pattern Pattern { get; private set; }
        public List<Projectile> PlayerProjectiles { get; private set; }

        public Game()
        {
            Player = new Player(new Vector2(_width / 2, _height * 5 / 6),
                29,
                54,
                4,
                -2);
            Enemy = new Enemy(new Vector2(_width / 2, _height / 6),
                29,
                54,
                4,
                1000);
            Pattern = new Pattern(50,
                10,
                Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
            PlayerProjectiles = new List<Projectile>();
        }

        public void Start()
        {
            _collisionTime = DateTime.Now;
            switch (DifficultyLevel)
            {
                case Difficulty.Easy:
                    Enemy.HP = 250;
                    Pattern = new Pattern(25, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                    ScoreMultiplier = 1;
                    break;
                case Difficulty.Medium:
                    Enemy.HP = 500;
                    Pattern = new Pattern(50, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                    ScoreMultiplier = 2;
                    break;
                case Difficulty.Hard:
                    Enemy.HP = 1250;
                    Pattern = new Pattern(125, 10, Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                    ScoreMultiplier = 5;
                    break;
                default:
                    break;
            }
        }

        public void MoveFires()
        {
            for (int i = 0; i < PlayerProjectiles.Count; i++)
            {
                PlayerProjectiles[i].Location.Y -= 7;
                if (PlayerProjectiles[i].Location.Y < 0) PlayerProjectiles.RemoveAt(i--);
            }
        }

        public int CheckFireCollision()
        {
            // Здесь наш классный алгоритм, который требовался
            int hits = PlayerProjectiles.RemoveAll(p => Math.Abs(p.Location.X - Enemy.Location.X) < p.Diameter
                    && Math.Abs(p.Location.Y - Enemy.Location.Y) < p.Diameter);
            Enemy.HP -= 10 * hits;

            // Теперь враг убегает от игрока при попадании
            if (hits > 0) Enemy.Speed = -Enemy.Speed;
            return hits;
        }

        public bool CheckCollision()
        {
            foreach (var projectile in Pattern.Projectiles)
            {
                if (projectile.Location.X < Player.Location.X + Player.Width
                    && projectile.Location.X + projectile.Diameter > Player.Location.X
                    && projectile.Location.Y < Player.Location.Y + Player.Height
                    && projectile.Location.Y + projectile.Diameter > Player.Location.Y)
                {
                    // Если после попадания не прошло 3 секунды, то повторного попадания нет
                    if ((DateTime.Now - _collisionTime).TotalMilliseconds > 3000)
                    {
                        _collisionTime = DateTime.Now;
                        if (--Player.ExtraLives == -1) Player.Die();
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveProjectiles(int elapsedTime)
        {
            if (elapsedTime % (150 * 15) == 0)
            {
                Pattern.SetToStartingPoint(Enemy.Location + new Vector2(Enemy.Width / 2, Enemy.Height));
                return;
            }
            Pattern.MoveProjectiles();
        }

        public void MoveEnemy()
        {
            if (Enemy.Location.X > _width * 3 / 4 || Enemy.Location.X + Enemy.Width < _width / 4) Enemy.Speed = -Enemy.Speed;
            Enemy.Location.X += Enemy.Speed;
        }

        public void SetPlayerToAction(HashSet<PlayerAction> actionSet, int elapsedTime)
        {
            int speed = ShiftIsDown ? Player.Speed + Player.ShiftModifier : Player.Speed;
            if (actionSet.Contains(PlayerAction.MoveUp) && Player.Location.Y > 0) Player.Location.Y -= speed;
            if (actionSet.Contains(PlayerAction.MoveLeft) && Player.Location.X > 0) Player.Location.X -= speed;
            if (actionSet.Contains(PlayerAction.MoveDown) && Player.Location.Y + Player.Height < _height) Player.Location.Y += speed;
            if (actionSet.Contains(PlayerAction.MoveRight) && Player.Location.X + Player.Width < _width) Player.Location.X += speed;
            if (actionSet.Contains(PlayerAction.Attack) && elapsedTime % 150 == 0) Player.Attack(PlayerProjectiles);
        }
    }
}