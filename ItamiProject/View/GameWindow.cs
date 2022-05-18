using System;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using Model;
using Controller;

namespace View
{
    public partial class GameWindow : Form
    {
        public GameWindow(Game game, GameController ctrl)
        {
            #region Window settings

            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(game.Width, game.Height);
            DoubleBuffered = true; // false -> рай эпилептика
            BackColor = Color.Black;
            Text = "Itami Project";

            // TODO: придумать что-то для поддержки разных разрешений(?)
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;

            #endregion

            #region Images

            string bgFolder = $@"{Environment.CurrentDirectory}\..\..\Resources\Backgrounds\";
            string charFolder = $@"{Environment.CurrentDirectory}\..\..\Resources\Characters\";
            string iconFolder = $@"{Environment.CurrentDirectory}\..\..\Resources\Icons\";

            Image playerImage = Image.FromFile($"{charFolder}maid_blue_front.png");
            Image playerImageTransparent = Image.FromFile($"{charFolder}maid_blue_front_transparent.png");
            Image enemyImage = Image.FromFile($"{charFolder}maid_blue_front_enemy.png");
            Image heartImage = Image.FromFile($"{iconFolder}heart.png");
            Image wastedImage = Image.FromFile($"{bgFolder}wasted.jpg");

            #endregion

            #region Sounds

            string soundsFolder = $@"{Environment.CurrentDirectory}\..\..\Resources\Sounds\";

            SoundPlayer menuSelectionSound = new SoundPlayer($"{soundsFolder}CURSOL_SELECT.wav");
            SoundPlayer menuOkSound = new SoundPlayer($"{soundsFolder}CURSOL_OK.wav");
            SoundPlayer playerIsHitSound = new SoundPlayer($"{soundsFolder}oof_sound.wav");

            #endregion

            #region Music

            string musicFolder = $@"{Environment.CurrentDirectory}\..\..\Resources\Music\";

            SoundPlayer simpleSound = new SoundPlayer($"{musicFolder}music.wav");
            bool isPlaying = false;

            #endregion

            #region Timer

            Timer gameTimer = new Timer();
            gameTimer.Interval = 15; // с интервалом >= 16 "лагает"
            gameTimer.Tick += (sender, e) =>
            {
                ctrl.IterateGameCycle();

                // По какой-то причине (SoundPlayer — помойка) останавливает проигрывание музыки
                if (ctrl.HasCollisionOccured()) playerIsHitSound.Play();

                if (game.Player.Lifes < 0)
                {
                    gameTimer.Stop();
                    MessageBox.Show("Your free trial of being alive has ended.", "Game Over");
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

            ComboBox resolutions = new ComboBox
            {
                Top = livesNumber.Bottom + 15,
                Left = buttonStart.Left,
                Width = 100
            };

            resolutions.Items.Add("1920 1080");
            resolutions.Items.Add("1280 720");
            resolutions.Items.Add("640 480");
            resolutions.SelectedItem = resolutions.Items[1];

            Controls.Add(introText);
            Controls.Add(buttonStart);
            Controls.Add(livesNumber);
            Controls.Add(resolutions);

            resolutions.SelectionChangeCommitted += (sender, e) =>
            {
                int w = int.Parse(resolutions.SelectedItem.ToString().Split()[0]);
                int h = int.Parse(resolutions.SelectedItem.ToString().Split()[1]);
                Size res = new Size(w, h);
                ClientSize = res;

            };

            buttonStart.Click += (sender, e) =>
            {
                menuOkSound.Play();
                buttonStart.Visible = false;
                introText.Visible = false;
                livesNumber.Visible = false;
                resolutions.Visible = false;
                ctrl.SetPlayerLivesNumber((int)livesNumber.Value);
                ctrl.StartGame();
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
                    if (game.ShiftIsDown) e.Graphics.DrawImage(playerImageTransparent, game.Player.Location.X, game.Player.Location.Y, game.Player.Width, game.Player.Height);
                    else e.Graphics.DrawImage(playerImage, game.Player.Location.X, game.Player.Location.Y, game.Player.Width, game.Player.Height);
                    e.Graphics.DrawImage(enemyImage, game.Enemy.Location.X, game.Enemy.Location.Y, game.Enemy.Width, game.Enemy.Height);
                    foreach (var projectile in game.Pattern.Projectiles)
                        e.Graphics.FillEllipse(Brushes.Red, projectile.Location.X, projectile.Location.Y, projectile.Hitbox, projectile.Hitbox);
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