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

            ClientSize = new Size(game.Width, game.Height);
            DoubleBuffered = true; // false -> рай эпилептика
            BackColor = Color.Black;
            Text = "Itami Project";
            Icon = new Icon($"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Icons\\ItamiIcon.ico");

            // TODO: придумать что-то для поддержки разных разрешений(?)
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            #endregion

            #region Images

            string bgFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Backgrounds\\";
            string charFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Characters\\";
            string iconFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Icons\\";

            Image playerImage = Image.FromFile($"{charFolder}maid_blue_front.png");
            Image playerImageTransparent = Image.FromFile($"{charFolder}maid_blue_front_transparent.png");
            Image enemyImage = Image.FromFile($"{charFolder}maid_blue_front_enemy.png");
            Image heartImage = Image.FromFile($"{iconFolder}heart.png");
            Image wastedImage = Image.FromFile($"{bgFolder}wasted.jpg");

            #endregion
            
            #region Sounds

            string soundsFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Sounds\\";

            SoundPlayer menuSelectionSound = new SoundPlayer($"{soundsFolder}CURSOL_SELECT.wav");
            SoundPlayer menuOkSound = new SoundPlayer($"{soundsFolder}CURSOL_OK.wav");

            #endregion

            #region Music

            string musicFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Music\\";

            SoundPlayer simpleSound = new SoundPlayer($"{musicFolder}music.wav");
            bool isPlaying = false;

            #endregion

            #region Timer

            Timer gameTimer = new Timer();
            gameTimer.Interval = 15; // с интервалом >= 16 "лагает"
            gameTimer.Tick += (sender, e) =>
            {
                ctrl.IterateGameCycle();
                if (game.Player.Lifes < 0)
                {
                    gameTimer.Stop();
                    MessageBox.Show("Возник нюанс — в Вас попали", "Game Over");
                }
                else Invalidate();
            };

            #endregion

            #region Form controls

            Label introText = new Label
            {
                AutoSize = true,
                Text = "Powered by Pain In The Lower Back\u2122 engine",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic),
                ForeColor = Color.OrangeRed,
                Left = 15,
                Top = 15
            };

            Button buttonStart = new Button
            {
                Size = new Size(180, 55),
                Text = "Начать игру?",
                Font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold),
                BackColor = Color.OrangeRed,
                ForeColor = Color.Black,
                Left = (ClientSize.Width - 180) / 2,
                Top = (ClientSize.Height - 55) / 2
            };

            NumericUpDown livesNumber = new NumericUpDown
            {
                Top = buttonStart.Bottom + 15,
                Left = buttonStart.Left,
                Minimum = 0,
                Maximum = 5,
                Value = 3,
                Width = buttonStart.Width
            };

            Controls.Add(introText);
            Controls.Add(buttonStart);
            Controls.Add(livesNumber);

            buttonStart.Click += (sender, e) =>
              {
                  menuOkSound.Play();
                  buttonStart.Visible = false;
                  introText.Visible = false;
                  livesNumber.Visible = false;
                  ctrl.SetPlayerLivesNumber((int)livesNumber.Value);
                  BackgroundImage = Image.FromFile($"{bgFolder}battleback8.png");
                  Focus(); // контролы любят забирать у формы фокус, и его надо отдавать обратно
                  gameTimer.Start();
              };
            buttonStart.MouseEnter += (sender, e) =>
              {
                  menuSelectionSound.Play();
                  Cursor = Cursors.Hand;
                  buttonStart.Text = "Начать игру?..";
                  buttonStart.BackColor = Color.Black;
                  buttonStart.ForeColor = Color.DarkRed;
              };
            buttonStart.MouseLeave += (sender, e) =>
              {
                  Cursor = Cursors.Default;
                  buttonStart.Text = "Начать игру?";
                  buttonStart.BackColor = Color.OrangeRed;
                  buttonStart.ForeColor = Color.Black;
              };

            #endregion

            #region Painting

            Paint += (sender, e) =>
            {
                if (gameTimer.Enabled)
                {
                    if (game.ShiftIsDown) e.Graphics.DrawImage(playerImageTransparent, game.Player.X, game.Player.Y, 29, 54);
                    else e.Graphics.DrawImage(playerImage, game.Player.X, game.Player.Y, 29, 54);
                    e.Graphics.DrawImage(enemyImage, game.Enemy.X, game.Enemy.Y, 29, 54);
                    foreach (var projectile in game.Pattern.Projectiles)
                        e.Graphics.FillEllipse(Brushes.Red, projectile.X, projectile.Y, projectile.Hitbox, projectile.Hitbox);
                    for (int i = -1; i < game.Player.Lifes - 1; i++)
                    {
                        e.Graphics.DrawImage(heartImage, 60 + (29 * i), 30, 29, 29);
                    }
                }
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
                  else if (e.KeyCode == Keys.M)
                  {
                      if (isPlaying)
                      {
                          simpleSound.Stop();
                          isPlaying = false;
                      }
                      else
                      {
                          simpleSound.PlayLooping();
                          isPlaying = true;
                      }
                  }
                  else if (e.KeyCode == Keys.ShiftKey) ctrl.HandleShift(true);
                  else ctrl.AddKeyToSet(e.KeyCode);
              };
            KeyUp += (sender, e) =>
              {
                  if (e.KeyCode == Keys.ShiftKey) ctrl.HandleShift(false);
                  else ctrl.RemoveKeyFromSet(e.KeyCode);
              };

            #endregion
        }
    }
}