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
            Size = new Size(1280, 720);
            string gamePath = Environment.CurrentDirectory;
            BackgroundImage = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Backgrounds\\battleback8.png"));
            Image characterImage = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Characters\\maid_blue_front.png"));
            Image characterImageTransparent = Image.FromFile(Path.Combine(gamePath, "..\\..\\", "Characters\\maid_blue_front_transparent.png"));
            byte speed = 7;
            bool moveUp = false;
            bool moveLeft = false;
            bool moveDown = false;
            bool moveRight = false;
            PictureBox charBox = new PictureBox();
            charBox.Image = characterImage;
            charBox.Location = new Point(Width / 2, Height / 4 * 3);
            charBox.Size = characterImage.Size;
            charBox.BackColor = Color.Transparent;
            Controls.Add(charBox);
            Timer gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Start();
            gameTimer.Tick += (sender, e) =>
            {
                if(moveUp && charBox.Top > 0) charBox.Top -= speed;
                if(moveLeft && charBox.Left > 0) charBox.Left -= speed;
                if(moveDown && charBox.Bottom < Height) charBox.Top += speed;
                if (moveRight && charBox.Right < Width) charBox.Left += speed;
                Invalidate();
            };
            Paint += (sender, e) =>
            {
            };
            KeyDown += (sender, e) =>
              {
                  switch (e.KeyCode)
                  {
                      case Keys.W:
                          moveUp = true;
                          break;
                      case Keys.A:
                          moveLeft = true;
                          break;
                      case Keys.S:
                          moveDown = true;
                          break;
                      case Keys.D:
                          moveRight = true;
                          break;
                      case Keys.ShiftKey:
                          speed = 4;
                          charBox.Image = characterImageTransparent;
                          break;
                  }
              };
            KeyUp += (sender, e) =>
              {
                  switch (e.KeyCode)
                  {
                      case Keys.W:
                          moveUp = false;
                          break;
                      case Keys.A:
                          moveLeft = false;
                          break;
                      case Keys.S:
                          moveDown = false;
                          break;
                      case Keys.D:
                          moveRight = false;
                          break;
                      case Keys.ShiftKey:
                          speed = 7;
                          charBox.Image = characterImage;
                          break;
                  }
              };
        }

        public static void Main()
        {
            Application.Run(new GameWindow { DoubleBuffered = true, Text = "Itami Project" });
        }
    }
}
