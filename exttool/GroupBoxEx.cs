using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace BirdsEye {
    /// <summary>
    /// A custom Windows control which extends <see cref="GroupBox"/>
    /// to add the ability to set the border color.
    /// </summary>
    public class GroupBoxEx : GroupBox {
        private Color _borderColor = Color.Black;
        public Color BorderColor {
            get { return _borderColor; }
            set { _borderColor = value; Invalidate(); }
        }
        private Color _textColor = Color.Black;
        public Color TextColor {
            get { return _textColor; }
            set { _textColor = value; Invalidate(); }
        }
        private Font _textFont = new(DefaultFont, FontStyle.Bold);
        public Font TextFont {
            get { return _textFont; }
            set { _textFont = value; Invalidate(); }
        }

        protected override void OnPaint(PaintEventArgs e) {
            Color titleColor = TextColor;
            DrawGroupBoxWithText(
                e.Graphics, new Rectangle(0, 0, Width, Height), Text, TextFont, titleColor
            );
            RaisePaintEvent(this, e);
        }

        private void DrawGroupBoxWithText(
            Graphics g, Rectangle bounds, string groupBoxText, Font font, Color titleColor
        ) {
            Rectangle rectangle = bounds;
            rectangle.Width -= 8;
            Size size = TextRenderer.MeasureText(
                g, groupBoxText, font, new Size(rectangle.Width, rectangle.Height)
            );
            rectangle.Width = size.Width;
            rectangle.Height = size.Height;
            rectangle.X += 8;
            TextRenderer.DrawText(g, groupBoxText, font, rectangle, titleColor);
            if (rectangle.Width > 0) {
                rectangle.Inflate(2, 0);
            }

            using var pen = new Pen(BorderColor, 2);
            int num = bounds.Top + font.Height / 2;
            g.DrawLine(pen, bounds.Left + 1, num - 1, bounds.Left + 1, bounds.Height - 2);
            g.DrawLine(pen, bounds.Left, bounds.Height - 2, bounds.Width - 1, bounds.Height - 2);
            g.DrawLine(pen, bounds.Left, num - 1, rectangle.X - 3, num - 1);
            g.DrawLine(pen, rectangle.X + rectangle.Width + 2, num - 1, bounds.Width - 2, num - 1);
            g.DrawLine(pen, bounds.Width - 2, num - 1, bounds.Width - 2, bounds.Height - 2);
        }
    }
}