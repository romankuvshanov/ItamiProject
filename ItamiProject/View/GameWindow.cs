using Model;
using Controller;
using System;
using System.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using ItamiProject.View;

namespace View
{
    public partial class GameWindow : Form
    {
        // нечто необходимое для игры
        private Game _game;
        private GameController _ctrl;
        private Timer _gameTimer = new Timer { Interval = 15 };
        private Dictionary<Keys, PlayerAction> _keysLayoutDict = new Dictionary<Keys, PlayerAction>();
        private Rectangle _gameArea;
        private Rectangle _statsArea;
        private Bitmap _defaultBg; // Пустая битмапа. Нужна для переопределённого BackgroundImage
        private bool _isPaused;
        private int _elapsedTime;

        #region **Всё, что связано со счётом

        private int _score;
        private int _hitCombo;
        private int _maxCombo;

        #endregion

        #region **Изображения

        private Image _bgImg = Image.FromFile(@"Resources\Backgrounds\battleback8.png");
        private Image _playerImg = Image.FromFile(@"Resources\Characters\maid_blue_front.png");
        private Image _playerImgTransp = Image.FromFile(@"Resources\Characters\maid_blue_front_transparent.png");
        private Image _enemyImg = Image.FromFile(@"Resources\Characters\maid_blue_front_enemy.png");
        private Image _heartImg = Image.FromFile(@"Resources\Textures\heart.png");
        private Image wastedImg = Image.FromFile(@"Resources\Backgrounds\wasted.jpg");

        #endregion

        #region **Звуки

        private SoundPlayer _menuSelectionSnd = new SoundPlayer(@"Resources\Sounds\CURSOL_SELECT.wav");
        private SoundPlayer _menuOkSnd = new SoundPlayer(@"Resources\Sounds\CURSOL_OK.wav");
        private SoundPlayer _playerIsHitSnd = new SoundPlayer(@"Resources\Sounds\oof_sound.wav");
        private SoundPlayer _defeatSound = new SoundPlayer(@"Resources\Sounds\wasted.wav");
        private SoundPlayer _winSound = new SoundPlayer(@"Resources\Sounds\you_win.wav");
        private SoundPlayer _music = new SoundPlayer(@"Resources\Music\music.wav");
        private bool _isMusicPlaying;

        #endregion

        #region **Контролы

        private Label _introLbl;
        private Button _btnStart;
        private Button _btnExit;
        private Button _btnLeaderboard;
        private NumericUpDown _livesNumericUD;
        private Label _livesNumberLbl;
        private ComboBox _difficultiesComboB;
        private Label _difficultyLbl;
        private ComboBox _keysLayoutComboB;
        private Label _keysLayoutLbl;
        private Label _scoreLbl;
        private Label _enemyHPLbl;
        private Label _pauseLbl;
        private Label _playerLivesLbl;
        private Label _comboLbl;
        private Label _ctrlTutorialLbl;
        private Button _btnHelp;

        #endregion

        public GameWindow()
        {
            _game = new Game();
            _ctrl = new GameController(_game);
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1024, 576);
            _gameArea = new Rectangle(
                ClientRectangle.Left,
                ClientRectangle.Top,
                ClientSize.Width / 4 * 3,
                ClientSize.Height);
            _statsArea = new Rectangle(
                _gameArea.Right,
                _gameArea.Top,
                ClientRectangle.Width - _gameArea.Width,
                ClientRectangle.Height);
            DoubleBuffered = true;
            Text = "Itami Project";
            BackColor = Color.Black;
            Icon = new Icon(@"Resources\Icons\ItamiIcon.ico");
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            InitializeControls();
            _gameTimer.Tick += TimerTick;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            _elapsedTime += _gameTimer.Interval;
            _ctrl.IterateGameCycle(_elapsedTime);
            if (_ctrl.WasEnemyHit())
            {
                _hitCombo++;
                _comboLbl.Text = $"Комбо: {_hitCombo}";
                if (_hitCombo > _maxCombo) _maxCombo = _hitCombo;
                _enemyHPLbl.Text = $"{"ХП врага: " + _game.Enemy.HP}";
                _score += 10;
                _scoreLbl.Text = $"Счёт: {_score:D10}";
            }
            if (_ctrl.WasPlayerHit())
            {
                _hitCombo = 0;
                _comboLbl.Text = $"Комбо: {_hitCombo}";
                DrawPlayersLives();
                _playerIsHitSnd.Play(); // По какой-то причине (SoundPlayer — помойка) останавливает проигрывание музыки
            }
            if (_game.Enemy.HP <= 0)
            {
                _gameTimer.Stop();
                _winSound.Play();
                var totalScore = _maxCombo * _game.ScoreMultiplier * (6 - (int)_livesNumericUD.Value) * _score;
                MessageBox.Show($"Чел, хорош!\n" +
                    $"Максимальное комбо: x{_maxCombo}\n" +
                    $"Множитель сложности: x{_game.ScoreMultiplier}\n" +
                    $"Множитель доп. жизней: x{6 - (int)_livesNumericUD.Value}\n" +
                    $"Итоговый счёт: {totalScore}",
                    "Перемога!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                HandleGameOver(totalScore);
            }
            else if (_game.Player.IsDead)
            {
                _gameTimer.Stop();
                ShowDefeatedScreen();
                _defeatSound.Play();
                var totalScore = _maxCombo * _game.ScoreMultiplier * (6 - (int)_livesNumericUD.Value) * _score;
                MessageBox.Show($"Броня не пробита.\n" +
                    $"Максимальное комбо: x{_maxCombo}\n" +
                    $"Множитель сложности: x{_game.ScoreMultiplier}\n" +
                    $"Множитель доп. жизней: x{6 - (int)_livesNumericUD.Value}\n" +
                    $"Итоговый счёт: {totalScore}",
                    "Поразка!",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
                HandleGameOver(totalScore);
            }
            else Invalidate(_gameArea);
        }

        private void HandleGameOver(int totalScore)
        {
            var f = new LeaderboardWindow(false, totalScore);
            f.ShowDialog();
            Application.Restart();
        }

        private void InitializeControls()
        {
            int horizontalPadding = 5;
            int verticalPadding = 10;
            _introLbl = new Label()
            {
                AutoSize = true,
                Text = "Powered by Pain In The Lower Back\u2122 engine.",
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic),
                BackColor = Color.Black,
                ForeColor = Color.OrangeRed,
                Left = 15,
                Top = 15
            };
            _btnStart = new Button()
            {
                Size = new Size(180, 55),
                Text = "Начать игру",
                Font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold),
                BackColor = Color.OrangeRed,
                ForeColor = Color.Black,
                Left = (ClientSize.Width - 180) / 2,
                Top = (ClientSize.Height - 55) / 2
            };
            _btnStart.Click += StartGame;
            _btnStart.MouseEnter += ButtonStart_MouseEnter;
            _btnStart.MouseLeave += ButtonStart_MouseLeave;
            _btnExit = new Button()
            {
                Size = _btnStart.Size,
                Text = "Выйти из игры",
                Font = _btnStart.Font,
                BackColor = _btnStart.BackColor,
                ForeColor = Color.SkyBlue,
                Left = _btnStart.Right + horizontalPadding,
                Top = _btnStart.Top
            };
            _btnExit.Click += (object sender, EventArgs e) => Application.Exit();
            _btnExit.MouseEnter += _btnExit_MouseEnter;
            _btnExit.MouseLeave += _btnExit_MouseLeave;
            _btnLeaderboard = new Button()
            {
                Size = _btnExit.Size,
                Text = "Таблица лидеров",
                Font = _btnStart.Font,
                BackColor = _btnStart.BackColor,
                ForeColor = Color.GreenYellow,
                Left = _btnStart.Left - horizontalPadding - _btnStart.Width,
                Top = _btnStart.Top
            };
            _btnLeaderboard.Click += _btnLeaderboard_Click;
            _btnLeaderboard.MouseEnter += _btnLeaderboard_MouseEnter;
            _btnLeaderboard.MouseLeave += _btnLeaderboard_MouseLeave;
            _livesNumericUD = new NumericUpDown()
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
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Underline),
                BackColor = Color.Black,
                ForeColor = Color.BlueViolet,
                Top = _livesNumericUD.Top,
                Left = _livesNumericUD.Right + horizontalPadding
            };
            _difficultiesComboB = new ComboBox()
            {
                Top = _livesNumericUD.Bottom + verticalPadding,
                Left = _livesNumericUD.Left,
                Width = 145,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _difficultiesComboB.Items.AddRange(new string[] { "Toddler", "Average 東方 enjoyer", "ZUN" });
            _difficultiesComboB.SelectedItem = _difficultiesComboB.Items[1];
            _difficultyLbl = new Label()
            {
                AutoSize = true,
                Text = "Сложность",
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Underline),
                BackColor = Color.Black,
                ForeColor = Color.BlueViolet,
                Top = _difficultiesComboB.Top,
                Left = _difficultiesComboB.Right + horizontalPadding
            };
            _keysLayoutComboB = new ComboBox()
            {
                Top = _difficultiesComboB.Bottom + verticalPadding,
                Left = _difficultiesComboB.Left,
                Width = 145,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _keysLayoutComboB.Items.AddRange(new string[] { "Классическая из 東方", "Современная" });
            _keysLayoutComboB.SelectedItem = _keysLayoutComboB.Items[0];
            _keysLayoutComboB.SelectionChangeCommitted += _keysLayoutComboB_SelectionChangeCommitted;
            _keysLayoutLbl = new Label()
            {
                AutoSize = true,
                Text = "Схема управления",
                Font = new Font(FontFamily.GenericSansSerif, 11, FontStyle.Underline),
                BackColor = Color.Black,
                ForeColor = Color.BlueViolet,
                Top = _keysLayoutComboB.Top,
                Left = _keysLayoutComboB.Right + horizontalPadding
            };
            _scoreLbl = new Label()
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                BackColor = Color.Black,
                ForeColor = Color.Orange,
                BorderStyle = BorderStyle.Fixed3D,
                Left = _statsArea.Left + horizontalPadding*2,
                Top = _statsArea.Top + verticalPadding,
                Visible = false
            };
            _enemyHPLbl = new Label()
            {
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                BackColor = Color.Black,
                ForeColor = Color.Red,
                BorderStyle = BorderStyle.Fixed3D,
                Left = _scoreLbl.Left,
                Top = _scoreLbl.Bottom + verticalPadding,
                Visible = false
            };
            _playerLivesLbl = new Label()
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                BackColor = Color.Black,
                ForeColor = Color.Aquamarine,
                BorderStyle = BorderStyle.Fixed3D,
                Text = "Жизни:",
                Left = _enemyHPLbl.Left,
                Top = _enemyHPLbl.Bottom + verticalPadding,
                Visible = false
            };
            _pauseLbl = new Label()
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold),
                BackColor = Color.Black,
                ForeColor = Color.Red,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "ПАУЗА",
                Visible = false,
                Left = _gameArea.Width / 2 - 75,
                Top = _gameArea.Height / 2 - 25
            };
            _comboLbl = new Label()
            {
                AutoSize = true,
                Font = new Font(FontFamily.GenericSansSerif, 14, FontStyle.Bold),
                BackColor = Color.Black,
                ForeColor = Color.Fuchsia,
                BorderStyle = BorderStyle.Fixed3D,
                Text = "Комбо: 0",
                Left = _playerLivesLbl.Left,
                Top = _playerLivesLbl.Bottom + verticalPadding,
                Visible = false
            };
            _ctrlTutorialLbl = new Label()
            {
                AutoSize = true,
                Text = "Движение: стрелки.\nАтака: Z.\nРежим точного движения: Shift",
                Left = _keysLayoutComboB.Left,
                Top = _keysLayoutComboB.Bottom + verticalPadding,
                Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Regular),
                BackColor = Color.Black,
                ForeColor = Color.White
            };
            _btnHelp = new Button()
            {
                Size = new Size(40, 40),
                Font = new Font(FontFamily.GenericSerif, 20, FontStyle.Bold),
                Text = "?",
                ForeColor = Color.White,
                BackColor = _btnStart.BackColor,
                Left = ClientRectangle.Right - 45,
                Top = ClientRectangle.Top + 5
            };
            _btnHelp.MouseEnter += _btnHelp_MouseEnter;
            _btnHelp.MouseLeave += _btnHelp_MouseLeave;
            _btnHelp.Click += _btnHelp_Click;
            Controls.Add(_introLbl);
            Controls.Add(_btnStart);
            Controls.Add(_btnExit);
            Controls.Add(_btnLeaderboard);
            Controls.Add(_livesNumericUD);
            Controls.Add(_livesNumberLbl);
            Controls.Add(_difficultiesComboB);
            Controls.Add(_difficultyLbl);
            Controls.Add(_keysLayoutComboB);
            Controls.Add(_keysLayoutLbl);
            Controls.Add(_scoreLbl);
            Controls.Add(_enemyHPLbl);
            Controls.Add(_playerLivesLbl);
            Controls.Add(_pauseLbl);
            Controls.Add(_comboLbl);
            Controls.Add(_ctrlTutorialLbl);
            Controls.Add(_btnHelp);
        }

        private void _btnHelp_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Вы с врагом атакуете друг друга снарядами.\n\n" +
                "Ваша задача — убить противника, уклоняясь от его снарядов.\n\n" +
                "Противник, так же как и Вы, умеет уклоняться!\n\n" +
                "Возможно, даже лучше, чем Вы.\n\n" +
                "Для более точного уклонения используйте клавишу режима\nточного движения. " +
                "Она заставляет Вашего персонажа\nдвигаться медленнее, а значит, более точно.\n\n" +
                "Меньшее кол-во начальных доп. жизней, более высокая\nсложность, а также " +
                "высокий показатель комбо повышают Ваш итоговый счёт!\n\n" +
                "GLHF!",
                "Как играть?",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void _btnHelp_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _btnHelp.BackColor = _btnStart.BackColor;
        }
        private void _btnHelp_MouseEnter(object sender, EventArgs e)
        {
            _menuSelectionSnd.Play();
            Cursor = Cursors.Hand;
            _btnHelp.BackColor = Color.Black;
        }
        private void _keysLayoutComboB_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if (_keysLayoutComboB.SelectedIndex == 0)
                _ctrlTutorialLbl.Text = "Движение: стрелки.\nАтака: Z.\nРежим точного движения: Shift";
            else
                _ctrlTutorialLbl.Text = "Движение: WASD.\nАтака: Space.\nРежим точного движения: Shift";
        }
        private void _btnLeaderboard_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _btnLeaderboard.BackColor = _btnStart.BackColor;
        }
        private void _btnLeaderboard_MouseEnter(object sender, EventArgs e)
        {
            _menuSelectionSnd.Play();
            Cursor = Cursors.Hand;
            _btnLeaderboard.BackColor = Color.Black;
        }

        private void _btnLeaderboard_Click(object sender, EventArgs e)
        {
            var f = new LeaderboardWindow(true);
            f.ShowDialog();
        }

        private void _btnExit_MouseLeave(object sender, EventArgs e)
        {
            Cursor = Cursors.Default;
            _btnExit.BackColor = _btnStart.BackColor;
        }

        private void _btnExit_MouseEnter(object sender, EventArgs e)
        {
            _menuSelectionSnd.Play();
            Cursor = Cursors.Hand;
            _btnExit.BackColor = Color.Black;
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
            _btnStart.Text = "Начать игру";
            _btnStart.BackColor = Color.OrangeRed;
            _btnStart.ForeColor = Color.Black;
        }

        private void CreateKeysLayout()
        {
            if (_keysLayoutComboB.SelectedIndex == 0)
            {
                _keysLayoutDict[Keys.Up] = PlayerAction.MoveUp;
                _keysLayoutDict[Keys.Left] = PlayerAction.MoveLeft;
                _keysLayoutDict[Keys.Down] = PlayerAction.MoveDown;
                _keysLayoutDict[Keys.Right] = PlayerAction.MoveRight;
                _keysLayoutDict[Keys.Z] = PlayerAction.Attack;
            }
            else if (_keysLayoutComboB.SelectedIndex == 1)
            {
                _keysLayoutDict[Keys.W] = PlayerAction.MoveUp;
                _keysLayoutDict[Keys.A] = PlayerAction.MoveLeft;
                _keysLayoutDict[Keys.S] = PlayerAction.MoveDown;
                _keysLayoutDict[Keys.D] = PlayerAction.MoveRight;
                _keysLayoutDict[Keys.Space] = PlayerAction.Attack;
            }
        }

        private void HideUnnecessaryControls()
        {
            _btnHelp.Visible = false;
            _ctrlTutorialLbl.Visible = false;
            _btnStart.Visible = false;
            _btnExit.Visible = false;
            _btnLeaderboard.Visible = false;
            _introLbl.Visible = false;
            _livesNumericUD.Visible = false;
            _difficultiesComboB.Visible = false;
            _keysLayoutComboB.Visible = false;
            _livesNumberLbl.Visible = false;
            _difficultyLbl.Visible = false;
            _keysLayoutLbl.Visible = false;
        }

        private void StartGame(object sender, EventArgs e)
        {
            _menuOkSnd.Play();
            CreateKeysLayout();
            HideUnnecessaryControls();
            _ctrl.StartGame((int)_livesNumericUD.Value, _difficultiesComboB.SelectedIndex);
            _scoreLbl.Text = $"Счёт: {0:D10}";
            _scoreLbl.Visible = true;
            _enemyHPLbl.Text = $"{"ХП врага: " + _game.Enemy.HP}";
            _enemyHPLbl.Visible = true; _playerLivesLbl.Visible = true;
            _enemyHPLbl.Size = _scoreLbl.Size;
            _comboLbl.Visible = true;
            BackgroundImage = _bgImg;
            DrawPlayersLives();
            Focus(); // <- забрать фокус у (какого-то) контрола, отдать форме
            _gameTimer.Start();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.P:
                    if (_gameTimer.Enabled
                        && !_isPaused)
                    {
                        _gameTimer.Stop();
                        _pauseLbl.Visible = true;
                        _isPaused = true;
                    }
                    else if (!_gameTimer.Enabled
                        && _isPaused)
                    {
                        _gameTimer.Start();
                        _pauseLbl.Visible = false;
                        _isPaused = false;
                    }
                    break;
                case Keys.Escape:
                    _gameTimer.Stop();
                    if (MessageBox.Show("Выйти из игры?", "Выйти?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes) Application.Exit();
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
                DrawPlayer(e);
                DrawPlayerProjectiles(e);
                DrawEnemy(e);
                DrawEnemyProjectiles(e);
            }
            else e.Graphics.FillRectangle(new SolidBrush(Color.Black), ClientRectangle);
        }
        private void DrawPlayer(PaintEventArgs e)
        {
            if (_game.ShiftIsDown) e.Graphics.DrawImage(_playerImgTransp, _game.Player.Location.X, _game.Player.Location.Y, _game.Player.Width, _game.Player.Height);
            else e.Graphics.DrawImage(_playerImg, _game.Player.Location.X, _game.Player.Location.Y, _game.Player.Width, _game.Player.Height);
        }
        private void DrawPlayerProjectiles(PaintEventArgs e)
        {
            foreach (var fireBall in _game.PlayerProjectiles)
                e.Graphics.FillEllipse(Brushes.Blue, fireBall.Location.X, fireBall.Location.Y, fireBall.Diameter, fireBall.Diameter);
        }
        private void DrawPlayersLives()
        {
            var g = CreateGraphics();
            g.FillRectangle(new SolidBrush(Color.Black), _playerLivesLbl.Right, _playerLivesLbl.Top, _statsArea.Width, _playerLivesLbl.Height);
            for (int i = 0; i < _game.Player.ExtraLives; i++)
                g.DrawImage(_heartImg, _playerLivesLbl.Right+5 + (_heartImg.Width*i), _playerLivesLbl.Top-2, _heartImg.Width, _heartImg.Height);
            g.Dispose();
        }
        private void DrawEnemy(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_enemyImg, _game.Enemy.Location.X, _game.Enemy.Location.Y, _game.Enemy.Width, _game.Enemy.Height);
        }
        private void DrawEnemyProjectiles(PaintEventArgs e)
        {
            foreach (var projectile in _game.Pattern.Projectiles)
                e.Graphics.FillEllipse(Brushes.Red, projectile.Location.X, projectile.Location.Y, projectile.Diameter, projectile.Diameter);
        }

        private void ShowDefeatedScreen()
        {
            _scoreLbl.Visible = false;
            _enemyHPLbl.Visible = false;
            _playerLivesLbl.Visible = false;
            _comboLbl.Visible = false;
            Graphics g = CreateGraphics();
            g.DrawImage(wastedImg, ClientRectangle);
            g.Dispose();
        }

        /*
         * Экспериментально. Возможно, повысит производительность (но это не точно).
         * 
         * NOTE: потестил с taskmanager, загрузка процессора падает в ~2.5 раза
         */
        public override Image BackgroundImage
        {
            get
            {
                return _defaultBg;
            }
            set
            {
                if (value != null)
                {
                    //Create new BitMap Object of the size 
                    _defaultBg = new Bitmap(value.Width, value.Height);

                    //Create graphics from image
                    System.Drawing.Graphics g =
                       System.Drawing.Graphics.FromImage(_defaultBg);

                    //set the graphics interpolation mode to high
                    g.InterpolationMode =
                       System.Drawing.Drawing2D.InterpolationMode.High;

                    //draw the image to the graphics to create the new image 
                    //which will be used in the onpaint background
                    g.DrawImage(value, _gameArea);
                }
                else
                    _defaultBg = null;
            }
        }
    }
}