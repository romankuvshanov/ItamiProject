using System;
using System.Drawing;
using System.Windows.Forms;
namespace GameView
{
    public partial class IntroControl : UserControl
    {
        public IntroControl(Size parentSize)
        {
            Size = parentSize;

            Label introText = new Label();
            introText.Text = "Powered by Pain In The Lower Back\u2122 engine";
            introText.Font = new Font(FontFamily.GenericMonospace, 10);
            introText.Width = 1000;
            introText.ForeColor = Color.White;
            introText.Left = (Width - introText.Width) / 2;
            introText.Top = (Height - introText.Height) / 2;
            Controls.Add(introText);
            KeyDown += (sender, e) =>
              {
                  if (e.KeyCode == Keys.Enter)
                  {
                      Enabled = false;
                      Hide();
                  }
              };
        }
    }
}
