using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ItamiProject
{
    public partial class GameWindow : Form
    {
        public GameWindow()
        {
            Game game = new Game();
            Point playerCoordinates = new Point();
            Point enemyCoordinates = new Point();

            // В этом листе бомбочки
            List<Point> enemyBombsCoordinates = new List<Point>();

            #region Form settings
            Size = new Size(game.width, game.height);
            MinimizeBox = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            #endregion

            #region Images
            string gamePath = Environment.CurrentDirectory;
            BackgroundImage = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Backgrounds\\battleback8.png"));
            Image playerImage = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Characters\\maid_blue_front.png"));
            Image playerImageTransparent = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Characters\\maid_blue_front_transparent.png"));
            Image enemyImage = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Characters\\maid_blue_front_monster.png"));
            Image enemyBomb = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Characters\\pile_of_poo.png"));
            #endregion

            Timer gameTimer = new Timer();
            gameTimer.Interval = 15; // с интервалом >= 16 "лагает"
            gameTimer.Start();
            gameTimer.Tick += (sender, e) =>
            {
                playerCoordinates = game.CheckForMovement();
                enemyCoordinates = game.MoveEnemy();
                enemyBombsCoordinates = game.UpdateEnemyBombsCoords();
                Invalidate();
            };
            Paint += (sender, e) =>
              {
                  if(!game.shiftIsDown) e.Graphics.DrawImage(playerImage, playerCoordinates);
                  else e.Graphics.DrawImage(playerImageTransparent, playerCoordinates);
                  e.Graphics.DrawImage(enemyImage, enemyCoordinates);

                  // Здесь я отрисовываю бомбочки
                  foreach (var point in enemyBombsCoordinates)
                      e.Graphics.DrawImage(enemyBomb, point);
                  
              };
            KeyDown += (sender, e) =>
              {
                  if (e.KeyCode == Keys.Escape)
                      if (MessageBox.Show("Do you want to quit the game?", "Quit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                          Close();
                  game.StartMovement(e.KeyCode);
              };
            KeyUp += (sender, e) =>
              {
                  game.StopMovement(e.KeyCode);
              };
        }

        public static void Main()
        {
            Application.Run(new GameWindow { DoubleBuffered = true, Text = "Itami Project" });
        }
    }
}