using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    public partial class Opaque : Form
    {
        public Opaque(string label1 = "Drag the mouse to select the interactive area of the game")
        {
            InitializeComponent();
            this.label1.Text = label1;
        }

        private void Opaque_Load(object sender, EventArgs e)
        {
            this.Size = new Size(label1.Size.Width + 10, label1.Size.Height + 10);
            Rectangle bounds = Helper.ScreenBounds();
            this.Location = new Point(bounds.Width / 2 - (this.Size.Width / 2), 0);
        }
    }
}
