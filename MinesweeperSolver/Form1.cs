using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private Rectangle ScreenBounds()
        {
            return Screen.GetBounds(Point.Empty);
        }
        private Bitmap CaptureScreen()
        {
            Rectangle bounds = ScreenBounds();
            using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                }
                return bitmap;
            }

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle bounds = ScreenBounds();
            this.Location = new Point(0, 0);
            this.Size = new Size(bounds.Width, bounds.Height);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            using (Bitmap screen = CaptureScreen())
            {
                //dosmth with screen
            }
        }
    }
}
