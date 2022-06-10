using NUnit.Framework;
using Model;
using View;
using Controller;
using System.Numerics;

namespace ItamiProject.Test
{
    [TestFixture]
    public class CharacterMovementTests
    {
        static Game game = new Game();
        static GameController ctrl = new GameController(game);

        [TestCase]
        public void CharactersMovesRightCorrectly()
        {
            var initialLocation = game.Player.Location;
            ctrl.HandleInput(PlayerAction.MoveRight, true);
            ctrl.IterateGameCycle(10);
            Assert.IsTrue(initialLocation.X < game.Player.Location.X, $"Was: {initialLocation.X}, now: {game.Player.Location.X}");
        }

        [TestCase]
        public void CharactersMovesUpCorrectly()
        {
            var initialLocation = game.Player.Location;
            ctrl.HandleInput(PlayerAction.MoveUp, true);
            ctrl.IterateGameCycle(10);
            Assert.IsTrue(initialLocation.Y > game.Player.Location.Y, $"Was: {initialLocation.Y}, now: {game.Player.Location.Y}");
        }

        [TestCase]
        public void CharactersMovesDownCorrectly()
        {
            var initialLocation = game.Player.Location;
            ctrl.HandleInput(PlayerAction.MoveDown, true);
            ctrl.IterateGameCycle(10);
            Assert.IsTrue(initialLocation.Y < game.Player.Location.Y, $"Was: {initialLocation.Y}, now: {game.Player.Location.Y}");
        }

        [TestCase]
        public void CharactersMovesLeftCorrectly()
        {
            var initialLocation = game.Player.Location;
            ctrl.HandleInput(PlayerAction.MoveLeft, true);
            ctrl.IterateGameCycle(10);
            Assert.IsTrue(initialLocation.X > game.Player.Location.X, $"Was: {initialLocation.X}, now: {game.Player.Location.X}");
        }
    }

    [TestFixture]
    public class CharacterActionsTests
    {
        static Game game = new Game();
        static GameController ctrl = new GameController(game);

        [TestCase]
        public void CharacterAttacksCorrectly()
        {
            Assert.IsTrue(game.PlayerProjectiles.Count == 0);
            game.Player.Attack(game.PlayerProjectiles);
            Assert.IsTrue(game.PlayerProjectiles.Count != 0);
        }

        [TestCase]
        public void ChacterSlowsDownCorrectly()
        {
            var initialLocation = game.Player.Location;
            ctrl.HandleInput(PlayerAction.MoveUp, true);
            ctrl.IterateGameCycle(1);
            var regularMovementDistance = initialLocation.Y - game.Player.Location.Y;

            game.ShiftIsDown = true;

            initialLocation = game.Player.Location;
            ctrl.HandleInput(PlayerAction.MoveUp, true);
            ctrl.IterateGameCycle(1);
            var slowMovementDistance = initialLocation.Y - game.Player.Location.Y;
            
            Assert.IsTrue(slowMovementDistance < regularMovementDistance, $"Slow distance: {slowMovementDistance}, regular distance: {regularMovementDistance}");
        }

        [TestCase]
        public void CharacterDiesCorrectly()
        {
            game.Player.Die();
            Assert.IsTrue(game.Player.IsDead);
        }
    }
    [TestFixture]
    public class GameDifficultyTests
    {
        static Game game;

        [TestCase]
        public void EasyDifficultyIsCorrect()
        {
            game = new Game();
            game.DifficultyLevel = Difficulty.Easy;
            game.Start();
            Assert.IsTrue(game.Enemy.HP == 250
                && game.Pattern.Projectiles.Length == 25
                && game.ScoreMultiplier == 1);
        }

        [TestCase]
        public void MediumDifficultyIsCorrect()
        {
            game = new Game();
            game.DifficultyLevel = Difficulty.Medium;
            game.Start();
            Assert.IsTrue(game.Enemy.HP == 500
                && game.Pattern.Projectiles.Length == 50
                && game.ScoreMultiplier == 2);
        }

        [TestCase]
        public void HardDifficultyIsCorrect()
        {
            game = new Game();
            game.DifficultyLevel = Difficulty.Hard;
            game.Start();
            Assert.IsTrue(game.Enemy.HP == 1250
                && game.Pattern.Projectiles.Length == 125
                && game.ScoreMultiplier == 5);
        }
    }

    [TestFixture]
    public class EnemyTests
    {
        static Game game;
        static GameController ctrl;

        [TestCase]
        public void EnemyMovesCorrectly()
        {
            game = new Game();
            ctrl = new GameController(game);
            var initialLocation = game.Enemy.Location;
            ctrl.IterateGameCycle(1);
            Assert.IsTrue(initialLocation != game.Enemy.Location);
        }

        [TestCase]
        public void EnemyShootsCorrectly()
        {
            game = new Game();
            ctrl = new GameController(game);
            var initialLocation = game.Pattern.Projectiles[0].Location;
            ctrl.IterateGameCycle(1);
            Assert.IsTrue(initialLocation != game.Pattern.Projectiles[0].Location);
        }
    }

    [TestFixture]
    public class PatternTests
    {
        static Game game;
        static GameController ctrl;

        [TestCase]
        public void PatternResetsCorrectly()
        {
            game = new Game();
            ctrl = new GameController(game);
            ctrl.IterateGameCycle(1);
            var positionAfterOneIteration = game.Pattern.Projectiles[0].Location;
            game.Pattern.SetToStartingPoint(game.Enemy.Location + new Vector2(game.Enemy.Width / 2, game.Enemy.Height));
            var positionAfterReset = game.Pattern.Projectiles[0].Location;
            Assert.IsTrue(positionAfterOneIteration != positionAfterReset);
        }
    }
}
