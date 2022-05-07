using System;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using GameModel;
using GameController;
namespace GameView
{
    public class GameWindow : Form
    {
        public GameWindow(Game game, Controller ctrl)
        {
            #region Window settings

            Width = game.Width;
            Height = game.Height;
            DoubleBuffered = true; // false -> рай эпилептика
            Text = "Itami Project";

            // TODO: придумать что-то для поддержки разных разрешений(?)
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            // FormBorderStyle.None позволяет правильно считать границы окна, чтобы игрок за них не выходил

            #endregion

            #region Images

            string bgFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Backgrounds\\";
            string charFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Characters\\";
            BackgroundImage = Image.FromFile($"{bgFolder}battleback8.png");
            Image playerImage = Image.FromFile($"{charFolder}maid_blue_front.png");
            Image playerImageTransparent = Image.FromFile($"{charFolder}maid_blue_front_transparent.png");
            Image enemyImage = Image.FromFile($"{charFolder}maid_blue_front_enemy.png");

            #endregion

            // Раскоментируйте, чтобы запустить музыку в игре
            # region Music

            //string musicFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Music\\";
            //SoundPlayer simpleSound = new SoundPlayer($"{musicFolder}music.wav");
            //simpleSound.PlayLooping();

            # endregion

            #region Timer

            Timer gameTimer = new Timer();
            gameTimer.Interval = 15; // с интервалом >= 16 "лагает"
            gameTimer.Start();
            gameTimer.Tick += (sender, e) =>
            {
                ctrl.InitiateMovement();
                Invalidate();
            };

            #endregion

            #region Painting

            SolidBrush projectileBrush = new SolidBrush(Color.Red);

            Paint += (sender, e) =>
            {
                if (game.shiftIsDown) e.Graphics.DrawImage(playerImageTransparent, game.player.x, game.player.y, 29, 54);
                else e.Graphics.DrawImage(playerImage, game.player.x, game.player.y, 29, 54);
                e.Graphics.DrawImage(enemyImage, game.enemy.x, game.enemy.y, 29, 54);
                foreach (var projectile in game.projectiles)
                    e.Graphics.FillEllipse(projectileBrush, projectile.x, projectile.y, projectile.hitbox, projectile.hitbox);
            };

            #endregion

            #region Input handling

            KeyDown += (sender, e) =>
              {
                  if (e.KeyCode == Keys.Escape)
                  {
                      gameTimer.Stop();
                      if (MessageBox.Show("Выйти из игры?", "Выйти?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                      == DialogResult.Yes)
                          Close();
                      else gameTimer.Start();
                  }
                  ctrl.AddKeyToSet(e.KeyCode);
              };
            KeyUp += (sender, e) =>
              {
                  ctrl.RemoveKeyFromSet(e.KeyCode);
              };

            #endregion
        }
    }
}