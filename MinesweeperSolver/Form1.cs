using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
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
            return Screen.GetBounds(System.Drawing.Point.Empty);
        }
        private Bitmap CaptureScreen()
        {
            Rectangle bounds = ScreenBounds();
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
            }
            return bitmap;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Rectangle bounds = ScreenBounds();
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new Size(bounds.Width, bounds.Height);
        }
        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.T)
            {
                //Bitmap _bitmap = CaptureScreen();
                //do something with _bitmap
                // _bitmap.save(@"C:\Users\user\Desktop\output.png"); //save resulting image to desktop
                MessageBox.Show("OK");
            }
        }
    }
}
