using System.Media;
using System.Drawing;
using System.Windows.Forms;
using Model;
using Controller;
using System;

namespace View
{
    public partial class GameWindow : Form
    {
        private Game game;
        private GameController ctrl;
        private Timer timer = new Timer { Interval = 15 };
        private int score;
        
        // изображения
        private Image playerImage = Image.FromFile(@"Resources\Characters\maid_blue_front.png");
        private Image playerImageTransparent = Image.FromFile(@"Resources\Characters\maid_blue_front_transparent.png");
        private Image enemyImage = Image.FromFile(@"Resources\Characters\maid_blue_front_enemy.png");
        private Image heartImage = Image.FromFile(@"Resources\Icons\heart.png");
        private Image wastedImage = Image.FromFile(@"Resources\Backgrounds\wasted.jpg");
        
        // звуки
        private SoundPlayer menuSelectionSound = new SoundPlayer(@"Resources\Sounds\CURSOL_SELECT.wav");
        private SoundPlayer menuOkSound = new SoundPlayer(@"Resources\Sounds\CURSOL_OK.wav");
        private SoundPlayer playerIsHitSound = new SoundPlayer(@"Resources\Sounds\oof_sound.wav");
        private SoundPlayer music = new SoundPlayer(@"Resources\Music\music.wav");
        private bool isMusicPlaying;

        // контролы
        private FlowLayoutPanel layoutPanel;
        private Label introLabel;
        private Button buttonStart;
        private NumericUpDown livesNumber;
        private ComboBox resolutions;
        private ComboBox difficultyLevels;
        private Label scoreLabel;
        private Label enemyHPLabel;
        private Label livesNumberLabel;
        private Label resolutionsLabel;
        private Label difficultyLevelsLabel;

        public GameWindow(Game game, GameController ctrl)
        {
            this.game = game;
            this.ctrl = ctrl;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(game.Width, game.Height);
            DoubleBuffered = true; // false -> рай эпилептика
            BackColor = Color.Black;
            Text = "Itami Project";
            Icon = new Icon(@"Resources\Icons\ItamiIcon.ico");
            // TODO: придумать что-то для поддержки разных разрешений(?)
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeControls();
            timer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            score += timer.Interval - 5;
            scoreLabel.Text = $"Score: {score:D10}";
            ctrl.IterateGameCycle();
            // По какой-то причине (SoundPlayer — помойка) останавливает проигрывание музыки
            if (ctrl.HasCollisionOccured()) playerIsHitSound.Play();
            if (game.Enemy.HP <= 0)
            {
                timer.Stop();
                MessageBox.Show("You won.", "Congratulations!");
            }
            else if (game.Player.Lives < 0)
            {
                timer.Stop();
                MessageBox.Show("Your free trial of being alive has ended.", "Game Over...");
            }
            else
            {
                enemyHPLabel.Text = $"{"Enemy HP:" + game.Enemy.HP}";
                Invalidate();
            }
        }
        private void InitializeControls()
        {
            layoutPanel = new FlowLayoutPanel()
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Right,
                Visible = false,
                // BackColor = Color.Transparent | Если сделать так, то начинает страшно лагать
            };
            introLabel = new Label
            {
                AutoSize = true,
                Text = "Powered by Pain In The Lower Back\u2122 engine",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic),
                ForeColor = Color.OrangeRed,
                Left = 15,
                Top = 15
            };
            buttonStart = new Button
            {
                Size = new Size(180, 55),
                Text = "Начать игру?",
                Font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold),
                BackColor = Color.OrangeRed,
                ForeColor = Color.Black,
                Left = (ClientSize.Width - 180) / 2,
                Top = (ClientSize.Height - 55) / 2
            };
            buttonStart.Click += StartGame;
            buttonStart.MouseEnter += ButtonStart_MouseEnter;
            buttonStart.MouseLeave += ButtonStart_MouseLeave;
            livesNumber = new NumericUpDown
            {
                Top = introLabel.Bottom + 15,
                Left = introLabel.Left,
                Minimum = 0,
                Maximum = 5,
                Value = 3,
                Width = 50
            };
            resolutions = new ComboBox
            {
                Top = livesNumber.Bottom + 15,
                Left = livesNumber.Left,
                Width = 80
            };
            resolutions.Items.AddRange(new string[] { "1920x1080", "1280x720", "640x480" });
            resolutions.SelectedItem = resolutions.Items[1];
            resolutions.SelectionChangeCommitted += Resolutions_SelectionChangeCommitted;
            difficultyLevels = new ComboBox()
            {
                Top = resolutions.Bottom + 15,
                Left = resolutions.Left,
                Width = 150
            };
            difficultyLevels.Items.AddRange(new string[] { "Toddler", "Average 東方 enjoyer", "ZUN" });
            difficultyLevels.SelectedItem = difficultyLevels.Items[1];
            scoreLabel = new Label()
            {
                AutoSize = true,
                Text = "Score: 0000000000",
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
            };
            enemyHPLabel = new Label()
            {
                AutoSize = true,
                Text = $"Enemy HP:{game.Enemy.HP}",
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
            };
            livesNumberLabel = new Label()
            {
                AutoSize = true,
                Text = "Кол-во жизней игрока",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = livesNumber.Top,
                Left = livesNumber.Right + 15
            };
            resolutionsLabel = new Label()
            {
                AutoSize = true,
                Text = "Разрешение",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = resolutions.Top,
                Left = resolutions.Right + 15
            };
            difficultyLevelsLabel = new Label()
            {
                AutoSize = true,
                Text = "Сложность",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = difficultyLevels.Top,
                Left = difficultyLevels.Right + 15
            };
            Controls.Add(layoutPanel);
            Controls.Add(introLabel);
            Controls.Add(buttonStart);
            Controls.Add(livesNumber);
            Controls.Add(resolutions);
            Controls.Add(difficultyLevels);
            layoutPanel.Controls.Add(scoreLabel);
            layoutPanel.Controls.Add(enemyHPLabel);
            Controls.Add(livesNumberLabel);
            Controls.Add(resolutionsLabel);
            Controls.Add(difficultyLevelsLabel);
        }
        private void Resolutions_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int w = int.Parse(resolutions.SelectedItem.ToString().Split('x')[0]);
            int h = int.Parse(resolutions.SelectedItem.ToString().Split('x')[1]);
            Size res = new Size(w, h);
            ClientSize = res;
        }
        private void ButtonStart_MouseEnter(object sender, EventArgs e)
        {
            menuSelectionSound.Play();
            Cursor = Cursors.Hand;
            buttonStart.Text = "Начать игру?..";
            buttonStart.BackColor = Color.Black;
            buttonStart.ForeColor = Color.DarkRed;
        }
        private void ButtonStart_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            buttonStart.Text = "Начать игру?";
            buttonStart.BackColor = Color.OrangeRed;
            buttonStart.ForeColor = Color.Black;
        }
        private void HideUnnecessaryControls()
        {
            buttonStart.Visible = false;
            introLabel.Visible = false;
            livesNumber.Visible = false;
            resolutions.Visible = false;
            difficultyLevels.Visible = false;
            livesNumberLabel.Visible = false;
            resolutionsLabel.Visible = false;
            difficultyLevelsLabel.Visible = false;
        }
        private void StartGame(object sender, EventArgs e)
        {
            menuOkSound.Play();
            HideUnnecessaryControls();
            layoutPanel.Visible = true;
            ctrl.SetPlayerLivesNumber((int)livesNumber.Value);
            ctrl.SetDifficultyLevel(difficultyLevels.SelectedIndex);
            ctrl.StartGame();
            BackgroundImage = Image.FromFile(@"Resources\Backgrounds\battleback8.png");
            Focus(); // контролы любят забирать у формы фокус, и его надо отдавать обратно
            timer.Start();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (e.KeyCode == Keys.Escape)
            {
                timer.Stop();
                if (MessageBox.Show("Выйти из игры?", "Выйти?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes)
                    Close();
                else timer.Start();
            }
            else if (e.KeyCode == Keys.M)
            {
                if (isMusicPlaying)
                {
                    music.Stop();
                    isMusicPlaying = false;
                }
                else
                {
                    music.PlayLooping();
                    isMusicPlaying = true;
                }
            }
            else if (e.KeyCode == Keys.ShiftKey) ctrl.HandleShift(true);
            else if (e.KeyCode == Keys.Space) ctrl.Attack();
            else ctrl.AddKeyToSet(e.KeyCode);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.ShiftKey) ctrl.HandleShift(false);
            else ctrl.RemoveKeyFromSet(e.KeyCode);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (timer.Enabled)
            {
                if (game.ShiftIsDown) e.Graphics.DrawImage(playerImageTransparent, game.Player.Location.X, game.Player.Location.Y, game.Player.Width, game.Player.Height);
                else e.Graphics.DrawImage(playerImage, game.Player.Location.X, game.Player.Location.Y, game.Player.Width, game.Player.Height);
                e.Graphics.DrawImage(enemyImage, game.Enemy.Location.X, game.Enemy.Location.Y, game.Enemy.Width, game.Enemy.Height);
                foreach (var projectile in game.Pattern.Projectiles)
                    e.Graphics.FillEllipse(Brushes.Red, projectile.Location.X, projectile.Location.Y, projectile.Hitbox, projectile.Hitbox);
                for (int i = -1; i < game.Player.Lives - 1; i++)
                {
                    e.Graphics.DrawImage(heartImage, 60 + (29 * i), 30, 29, 29);
                }
                foreach (var fireBall in game.PlayerProjectiles)
                {
                    e.Graphics.FillEllipse(Brushes.Blue, fireBall.Location.X, fireBall.Location.Y, fireBall.Hitbox, fireBall.Hitbox);
                }
            }
        }
    }
}