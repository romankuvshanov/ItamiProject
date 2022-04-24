using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ItamiProject
{
    public partial class GameWindow : Form
    {
        public GameWindow()
        {
            #region
            Size = new Size(1280, 720);
            string gamePath = Environment.CurrentDirectory;
            BackgroundImage = Image.FromFile(Path.Combine(gamePath, "Backgrounds\\battleback8.png"));
            Image characterImage = Image.FromFile(Path.Combine(gamePath, "Characters\\maid_blue_front.png"));
            int playerX = Width / 2;
            int playerY = Height / 4 * 3;
            int verticalEnemiesAmount = 50;
            Rectangle[] verticalEnemies = new Rectangle[verticalEnemiesAmount];
            int horizontalEnemiesAmount = 10;
            Rectangle[] horizontalEnemies = new Rectangle[horizontalEnemiesAmount];
            int offset = 10;
            for (int i = 0; i < verticalEnemiesAmount; i++)
            {
                verticalEnemies[i] = new Rectangle(offset, 0, 15, 15);
                offset += 70;
            }
            offset = 10;
            for (int i = 0; i < horizontalEnemiesAmount; i++)
            {
                horizontalEnemies[i] = new Rectangle(Width, offset, 20, 20);
                offset += 120;
            }
            Timer gameTimer = new Timer();
            gameTimer.Interval = 10;
            gameTimer.Start();
            gameTimer.Tick += (sender, e) =>
            {
                for (int i = 0; i < verticalEnemiesAmount; i++)
                {
                    if (verticalEnemies[i].Y > Height) verticalEnemies[i].Y = -15;
                    else verticalEnemies[i].Offset(0, 5);
                }
                for (int i = 0; i < horizontalEnemiesAmount; i++)
                {
                    if (horizontalEnemies[i].X < -20) horizontalEnemies[i].X = Width;
                    else horizontalEnemies[i].Offset(-8, 0);
                }
                Invalidate();
            };
            Paint += (sender, e) =>
            {
                e.Graphics.DrawRectangles(new Pen(Color.OrangeRed), verticalEnemies);
                e.Graphics.DrawRectangles(new Pen(Color.BlueViolet), horizontalEnemies);
                e.Graphics.DrawImage(characterImage, playerX, playerY);
            };
            KeyDown += (sender, e) =>
              {
                  switch (e.KeyCode)
                  {
                      case Keys.W:
                          if (playerY >= 0) playerY -= 15;
                          break;
                      case Keys.A:
                          if (playerX > 10) playerX -= 15;
                          break;
                      case Keys.S:
                          if (playerY < Height - 50) playerY += 15;
                          break;
                      case Keys.D:
                          if (playerX < Width - 50) playerX += 15;
                          break;
                  }
              };
            #endregion
        }

        public static void Main()
        {
            Application.Run(new GameWindow { DoubleBuffered = true, Text = "Itami Project" });
        }
    }
}
