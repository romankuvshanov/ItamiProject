using System;
using System.Drawing;
using System.Windows.Forms;

namespace ItamiProject
{
    internal class Game
    {
        bool moveUp = false;
        bool moveLeft = false;
        bool moveDown = false;
        bool moveRight = false;
        public bool shiftIsDown = false;
        byte speedWithoutShift = 7;
        byte speedWithShift = 4;
        public int width, height;
        Player player;

        public Game()
        {
            width = 1280;
            height = 720;
            player = new Player(width / 2, height * 3 / 4);
        }

        public void StartMovement(Keys pressedKey)
        {
            switch (pressedKey)
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
                    shiftIsDown = true;
                    break;
            }
        }

        public void StopMovement(Keys pressedKey)
        {
            switch (pressedKey)
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
                    shiftIsDown = false;
                    break;
            }
        }

        public Point CheckForMovement()
        {
            if (moveUp && player.y > 0) player.y -= speedWithoutShift;
            if (moveLeft && player.x > 0) player.x -= speedWithoutShift;
            if (moveDown && player.y + 54 < height) player.y += speedWithoutShift;
            if (moveRight && player.x + 29 < width) player.x += speedWithoutShift;
            return new Point(player.x, player.y);
        }
    }
}