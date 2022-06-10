using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ItamiProject.View
{
    public partial class LeaderboardWindow : Form
    {
        private TextBox _name;
        private Button _confirmButton;
        private DataGridView _board;
        private int _score;
        private TextBox _scoreTB;

        public LeaderboardWindow(bool isReadonly, int score = 0)
        {
            InitializeComponent();
            Text = "Таблица лидеров";
            Icon = new Icon(@"Resources\Icons\ItamiIcon.ico");
            InitializeControls();
            if (isReadonly)
            {
                _name.Visible = false;
                _scoreTB.Visible = false;
                _confirmButton.Visible = false;
            }
            else
            {
                _name.Visible = true;
                _scoreTB.Visible = true;
                _confirmButton.Visible = true;
            }
            BackColor = Color.Black;
            ClientSize = new Size(403, 274);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            CenterToParent();
            LoadLeaderboard();
            _score = score;
            _scoreTB.Text = $"{_score}";
        }

        private void LoadLeaderboard()
        {
            _board.Rows.Clear();
            List<string[]> lines = System.IO.File.ReadAllLines(@"Resources\Leaderboard.txt")
                .Select(line => line.Split())
                .OrderByDescending(splited => int.Parse(splited[1]))
                .ToList();
            foreach (var line in lines) _board.Rows.Add(line);
        }
        private void InitializeControls()
        {
            _name = new TextBox()
            {
                Width = 150,
                Location = new Point(0, 5)
            };
            _confirmButton = new Button()
            {
                AutoSize = true,
                Text = "Подтвердить",
                Left = _name.Right + 1,
                Top = _name.Top,
                BackColor = Color.DarkGreen,
                ForeColor = Color.Orange
            };
            _confirmButton.Click += _confirmButton_Click;
            _scoreTB = new TextBox()
            {
                Width = 100,
                Height = _name.Height,
                Left = _confirmButton.Right+7,
                Top = _name.Top,
                Enabled = false
            };
            _board = new DataGridView()
            {
                Top = _name.Bottom + 5,
                Enabled = false,
                RowHeadersVisible = false,
                Width = 403,
                Height = 600,
                AllowUserToAddRows = false,
            };
            _board.Columns.Add("PlayerName", "Имя игрока");
            _board.Columns.Add("PlayerScore", "Счёт");
            _board.Columns[0].Width = 200;
            _board.Columns[1].Width = 200;
            Controls.Add(_name);
            Controls.Add(_confirmButton);
            Controls.Add(_board);
            Controls.Add(_scoreTB);
        }
        private void _confirmButton_Click(object sender, EventArgs e)
        {
            if (_name == null || _name.Text == "")
            {
                MessageBox.Show("Введите имя! (пожалуйста)",
                    "Occured нюанс",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }
            _name.Enabled = false;
            _confirmButton.Enabled = false;

            /*
             * 1. сформироать новую запись
             * 2. добавить её в лист 
             * 3. считать в лист записи с доски
             * 4. отсортировать лист
             * 5. взять первые 10
             * 6. записать в файл
             * 7. обновить доску
             */

            List<string> lines = new List<string>();
            lines.Add(_name.Text + " " + _score);
            for (int i = 0; i < _board.Rows.Count; i++)
            {
                var name = _board.Rows[i].Cells[0].Value.ToString();
                var score = _board.Rows[i].Cells[1].Value.ToString();
                lines.Add(name + " " + score);
            }
            lines = lines
                .OrderByDescending(line => int.Parse(line.Split()[1]))
                .Take(10)
                .ToList();
            System.IO.File.WriteAllLines(@"Resources\Leaderboard.txt", lines);
            LoadLeaderboard();
        }
    }
}