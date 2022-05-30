using Model;
using Controller;
using System;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace View
{
    public partial class GameWindow : Form
    {
        // нечто необходимое для игры
        private Game _game;
        private GameController _ctrl;
        private Timer _gameTimer = new Timer { Interval = 15 };
        private int _score;
        private Dictionary<Keys, PlayerAction> _keysLayoutDict = new Dictionary<Keys, PlayerAction>();
        
        // изображения
        private Image _bgImg = Image.FromFile(@"Resources\Backgrounds\battleback8.png");
        private Image _playerImg = Image.FromFile(@"Resources\Characters\maid_blue_front.png");
        private Image _playerImgTransp = Image.FromFile(@"Resources\Characters\maid_blue_front_transparent.png");
        private Image _enemyImg = Image.FromFile(@"Resources\Characters\maid_blue_front_enemy.png");
        private Image _heartImg = Image.FromFile(@"Resources\Textures\heart.png");
        //private Image wastedImage = Image.FromFile(@"Resources\Backgrounds\wasted.jpg");
        
        // звуки
        private SoundPlayer _menuSelectionSnd = new SoundPlayer(@"Resources\Sounds\CURSOL_SELECT.wav");
        private SoundPlayer _menuOkSnd = new SoundPlayer(@"Resources\Sounds\CURSOL_OK.wav");
        private SoundPlayer _playerIsHitSnd = new SoundPlayer(@"Resources\Sounds\oof_sound.wav");
        private SoundPlayer _music = new SoundPlayer(@"Resources\Music\music.wav");
        private bool _isMusicPlaying;

        // контролы
        private Label _introLbl;
        private Button _btnStart;
        private NumericUpDown _livesNumericUD;
        private Label _livesNumberLbl;
        private ComboBox _resolutionsComboB;
        private Label _resolutionsLbl;
        private ComboBox _difficultiesComboB;
        private Label _difficultyLbl;
        private ComboBox _keysLayoutComboB;
        private Label _keysLayoutLbl;
        private FlowLayoutPanel _flowLayoutPanel;
        private Label _scoreLbl;
        private Label _enemyHPLbl;

        public GameWindow()
        {
            _game = new Game();
            _ctrl = new GameController(_game);
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1280, 720); // NOTE: так и должно быть. Отвязываем разрешение от размера "игрового поля"
            DoubleBuffered = true; // NOTE: false -> рай эпилептика
            BackColor = Color.Black;
            Text = "Itami Project";
            Icon = new Icon(@"Resources\Icons\ItamiIcon.ico");
            // TODO: придумать что-то для поддержки разных разрешений(?)
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeControls();
            _gameTimer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _score += (_gameTimer.Interval - 5) * _game.ScoreMultiplier;
            _scoreLbl.Text = $"Score: {_score:D10}";
            _ctrl.IterateGameCycle();
            if (_ctrl.WasPlayerHit()) _playerIsHitSnd.Play(); // По какой-то причине (SoundPlayer — помойка) останавливает проигрывание музыки
            if (_ctrl.WasEnemyHit()) _enemyHPLbl.Text = $"{"Enemy HP:" + _game.Enemy.HP}";
            if (_game.Enemy.HP <= 0)
            {
                _gameTimer.Stop();
                MessageBox.Show("You won.", "Congratulations!");
            }
            else if (_game.Player.IsDead)
            {
                _gameTimer.Stop();
                MessageBox.Show("Your free trial of being alive has ended.", "Game Over...");
            }
            else Invalidate();
        }
        private void InitializeControls()
        {
            int horizontalPadding = 5;
            int verticalPadding = 10;
            _introLbl = new Label
            {
                AutoSize = true,
                Text = "Powered by Pain In The Lower Back\u2122 engine.",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic),
                ForeColor = Color.OrangeRed,
                Left = 15,
                Top = 15
            };
            _btnStart = new Button
            {
                Size = new Size(180, 55),
                Text = "Начать игру?",
                Font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold),
                BackColor = Color.OrangeRed,
                ForeColor = Color.Black,
                Left = (ClientSize.Width - 180) / 2,
                Top = (ClientSize.Height - 55) / 2
            };
            _btnStart.Click += StartGame;
            _btnStart.MouseEnter += ButtonStart_MouseEnter;
            _btnStart.MouseLeave += ButtonStart_MouseLeave;
            _livesNumericUD = new NumericUpDown
            {
                Top = _introLbl.Bottom + verticalPadding,
                Left = _introLbl.Left,
                Minimum = 0,
                Maximum = 5,
                Value = 3,
                Width = 50,
                ReadOnly = true
            };
            _livesNumberLbl = new Label()
            {
                AutoSize = true,
                Text = "Доп. жизни",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = _livesNumericUD.Top,
                Left = _livesNumericUD.Right + horizontalPadding
            };
            _resolutionsComboB = new ComboBox
            {
                Top = _livesNumericUD.Bottom + verticalPadding,
                Left = _livesNumericUD.Left,
                Width = 90,
                DropDownStyle = ComboBoxStyle.DropDownList,
            };
            _resolutionsComboB.Items.AddRange(new string[] { "1920x1080", "1280x720", "640x480" });
            _resolutionsComboB.SelectedItem = _resolutionsComboB.Items[1];
            _resolutionsComboB.SelectionChangeCommitted += Resolutions_SelectionChangeCommitted;
            _resolutionsLbl = new Label()
            {
                AutoSize = true,
                Text = "Разрешение",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = _resolutionsComboB.Top,
                Left = _resolutionsComboB.Right + horizontalPadding
            };
            _difficultiesComboB = new ComboBox()
            {
                Top = _resolutionsComboB.Bottom + verticalPadding,
                Left = _resolutionsComboB.Left,
                Width = 145,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _difficultiesComboB.Items.AddRange(new string[] { "Toddler", "Average 東方 enjoyer", "ZUN" });
            _difficultiesComboB.SelectedItem = _difficultiesComboB.Items[1];
            _difficultyLbl = new Label()
            {
                AutoSize = true,
                Text = "Сложность",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = _difficultiesComboB.Top,
                Left = _difficultiesComboB.Right + horizontalPadding
            };
            _keysLayoutComboB = new ComboBox()
            {
                Top = _difficultiesComboB.Bottom + verticalPadding,
                Left = _difficultiesComboB.Left,
                Width = 100,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _keysLayoutComboB.Items.AddRange(new string[] { "東方 Classic", "Modern" });
            _keysLayoutComboB.SelectedItem = _keysLayoutComboB.Items[0];
            _keysLayoutLbl = new Label()
            {
                AutoSize = true,
                Text = "Схема управления",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Underline),
                ForeColor = Color.BlueViolet,
                Top = _keysLayoutComboB.Top,
                Left = _keysLayoutComboB.Right + horizontalPadding
            };
            _flowLayoutPanel = new FlowLayoutPanel()
            {
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                Dock = DockStyle.Right,
                Visible = false
                // BackColor = Color.Transparent | Если сделать так, то начинает страшно лагать
            };
            _scoreLbl = new Label()
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                ForeColor = Color.Orange,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
            };
            _enemyHPLbl = new Label()
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.Black,
                BorderStyle = BorderStyle.FixedSingle,
            };
            Controls.Add(_introLbl);
            Controls.Add(_btnStart);
            Controls.Add(_livesNumericUD);
            Controls.Add(_livesNumberLbl);
            Controls.Add(_resolutionsComboB);
            Controls.Add(_resolutionsLbl);
            Controls.Add(_difficultiesComboB);
            Controls.Add(_difficultyLbl);
            Controls.Add(_keysLayoutComboB);
            Controls.Add(_keysLayoutLbl);
            Controls.Add(_flowLayoutPanel);
            _flowLayoutPanel.Controls.Add(_scoreLbl);
            _flowLayoutPanel.Controls.Add(_enemyHPLbl);
        }
        private void Resolutions_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int w = int.Parse(_resolutionsComboB.SelectedItem.ToString().Split('x')[0]);
            int h = int.Parse(_resolutionsComboB.SelectedItem.ToString().Split('x')[1]);
            Size res = new Size(w, h);
            ClientSize = res;
        }
        private void ButtonStart_MouseEnter(object sender, EventArgs e)
        {
            _menuSelectionSnd.Play();
            Cursor = Cursors.Hand;
            _btnStart.Text = "Начать игру?..";
            _btnStart.BackColor = Color.Black;
            _btnStart.ForeColor = Color.DarkRed;
        }
        private void ButtonStart_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _btnStart.Text = "Начать игру?";
            _btnStart.BackColor = Color.OrangeRed;
            _btnStart.ForeColor = Color.Black;
        }
        private void CreateKeysLayout()
        {
            if (_keysLayoutComboB.SelectedIndex == 0)
            {
                _keysLayoutDict[Keys.Up] = PlayerAction.MoveU;
                _keysLayoutDict[Keys.Left] = PlayerAction.MoveL;
                _keysLayoutDict[Keys.Down] = PlayerAction.MoveD;
                _keysLayoutDict[Keys.Right] = PlayerAction.MoveR;
                _keysLayoutDict[Keys.Z] = PlayerAction.Attack;
            }
            else if (_keysLayoutComboB.SelectedIndex == 1)
            {
                _keysLayoutDict[Keys.W] = PlayerAction.MoveU;
                _keysLayoutDict[Keys.A] = PlayerAction.MoveL;
                _keysLayoutDict[Keys.S] = PlayerAction.MoveD;
                _keysLayoutDict[Keys.D] = PlayerAction.MoveR;
                _keysLayoutDict[Keys.Space] = PlayerAction.Attack;
            }
        }
        private void HideUnnecessaryControls()
        {
            _btnStart.Visible = false;
            _introLbl.Visible = false;
            _livesNumericUD.Visible = false;
            _resolutionsComboB.Visible = false;
            _difficultiesComboB.Visible = false;
            _keysLayoutComboB.Visible = false;
            _livesNumberLbl.Visible = false;
            _resolutionsLbl.Visible = false;
            _difficultyLbl.Visible = false;
            _keysLayoutLbl.Visible = false;
        }
        private void StartGame(object sender, EventArgs e)
        {
            _menuOkSnd.Play();
            CreateKeysLayout();
            HideUnnecessaryControls();
            _flowLayoutPanel.Visible = true;
            _ctrl.StartGame((int)_livesNumericUD.Value, _difficultiesComboB.SelectedIndex);
            _enemyHPLbl.Text = $"{"Enemy HP:" + _game.Enemy.HP}";
            BackgroundImage = _bgImg;
            Focus(); // контролы любят забирать у формы фокус, и его надо отдавать обратно
            _gameTimer.Start();
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.P:
                    // TODO: ДОДЕЛАТЬ
                    if(_gameTimer.Enabled) _gameTimer.Stop();
                    else _gameTimer.Start();
                    break;
                case Keys.Escape:
                    _gameTimer.Stop();
                    if (MessageBox.Show("Выйти из игры?", "Выйти?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes) Close();
                    else _gameTimer.Start();
                    break;
                case Keys.M:
                    if (_isMusicPlaying)
                    {
                        _music.Stop();
                        _isMusicPlaying = false;
                    }
                    else
                    {
                        _music.PlayLooping();
                        _isMusicPlaying = true;
                    }
                    break;
                case Keys.ShiftKey:
                    _ctrl.HandleShift(true);
                    break;
                default:
                    break;
            }
            if (_keysLayoutDict.ContainsKey(e.KeyCode)) _ctrl.HandleInput(_keysLayoutDict[e.KeyCode], true);
        }
        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            if (e.KeyCode == Keys.ShiftKey) _ctrl.HandleShift(false);
            else if (_keysLayoutDict.ContainsKey(e.KeyCode)) _ctrl.HandleInput(_keysLayoutDict[e.KeyCode], false);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_gameTimer.Enabled)
            {
                if (_game.ShiftIsDown) e.Graphics.DrawImage(_playerImgTransp, _game.Player.Location.X, _game.Player.Location.Y, _game.Player.Width, _game.Player.Height);
                else e.Graphics.DrawImage(_playerImg, _game.Player.Location.X, _game.Player.Location.Y, _game.Player.Width, _game.Player.Height);
                e.Graphics.DrawImage(_enemyImg, _game.Enemy.Location.X, _game.Enemy.Location.Y, _game.Enemy.Width, _game.Enemy.Height);
                foreach (var projectile in _game.Pattern.Projectiles)
                    e.Graphics.FillEllipse(Brushes.Red, projectile.Location.X, projectile.Location.Y, projectile.Diameter, projectile.Diameter);
                for (int i = -1; i < _game.Player.ExtraLives - 1; i++)
                {
                    e.Graphics.DrawImage(_heartImg, 60 + (_heartImg.Width * i), 30, _heartImg.Width, _heartImg.Height);
                }
                foreach (var fireBall in _game.PlayerProjectiles)
                {
                    e.Graphics.FillEllipse(Brushes.Blue, fireBall.Location.X, fireBall.Location.Y, fireBall.Diameter, fireBall.Diameter);
                }
            }
        }
    }
}