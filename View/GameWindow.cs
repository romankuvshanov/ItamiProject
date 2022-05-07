﻿using System;
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
            Image playerImage = Image.FromFile($"{charFolder}maid_blue_front.png");
            Image playerImageTransparent = Image.FromFile($"{charFolder}maid_blue_front_transparent.png");
            Image enemyImage = Image.FromFile($"{charFolder}maid_blue_front_enemy.png");

            #endregion

            #region Timer

            Timer gameTimer = new Timer();
            gameTimer.Interval = 15; // с интервалом >= 16 "лагает"
            gameTimer.Tick += (sender, e) =>
            {
                ctrl.IterateGameCycle();
                if (game.IsCollisionOccured)
                {
                    gameTimer.Stop();
                    MessageBox.Show("Возник нюанс — в Вас попали", "Game Over");
                }
                else Invalidate();
            };

            #endregion

            #region Form controls

            Label introText = new Label();
            introText.AutoSize = true;
            introText.Text = "Powered by Pain In The Lower Back\u2122 engine";
            introText.Font = new Font(FontFamily.GenericSansSerif, 10, FontStyle.Italic);
            introText.ForeColor = Color.OrangeRed;
            introText.Left = 15;
            introText.Top = 15;

            Button buttonStart = new Button();
            buttonStart.Size = new Size(180, 55);
            buttonStart.Text = "Начать игру?";
            buttonStart.Font = new Font(FontFamily.GenericMonospace, 12, FontStyle.Bold);
            buttonStart.BackColor = Color.OrangeRed;
            buttonStart.ForeColor = Color.Black;
            buttonStart.Left = (ClientSize.Width - buttonStart.Width) / 2;
            buttonStart.Top = (ClientSize.Height - buttonStart.Height) / 2;
            buttonStart.Click += (sender, e) =>
              {
                  buttonStart.Visible = false;
                  introText.Visible = false;
                  BackgroundImage = Image.FromFile($"{bgFolder}battleback8.png");
                  Focus(); // контролы любят забирать у формы фокус, и его надо отдавать обратно
                  gameTimer.Start();
              };
            buttonStart.MouseEnter += (sender, e) =>
              {
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

            Controls.Add(introText);
            Controls.Add(buttonStart);

            #endregion

            // Раскоментируйте, чтобы запустить музыку в игре
            # region Music

            //string musicFolder = $"{Environment.CurrentDirectory}\\..\\..\\..\\View\\_Music\\";
            //SoundPlayer simpleSound = new SoundPlayer($"{musicFolder}music.wav");
            //simpleSound.PlayLooping();

            # endregion

            #region Painting

            SolidBrush projectileBrush = new SolidBrush(Color.Red);

            Paint += (sender, e) =>
            {
                if (gameTimer.Enabled)
                {
                    if (game.shiftIsDown) e.Graphics.DrawImage(playerImageTransparent, game.player.x, game.player.y, 29, 54);
                    else e.Graphics.DrawImage(playerImage, game.player.x, game.player.y, 29, 54);
                    e.Graphics.DrawImage(enemyImage, game.enemy.x, game.enemy.y, 29, 54);
                    foreach (var projectile in game.projectiles)
                        e.Graphics.FillEllipse(projectileBrush, projectile.x, projectile.y, projectile.hitbox, projectile.hitbox);
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