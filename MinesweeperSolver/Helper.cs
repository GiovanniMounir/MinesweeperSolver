using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MinesweeperSolver
{
    class Helper
    {
        //DPI Check
        //DLL import
        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        //get windows DPI
        private static float getScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

        //checks DPI is 100%
        public static Rectangle ScreenBounds()
        {
            if (getScalingFactor() != 1)
            {
                MessageBox.Show("This program does not work with a scaling factor (DPI) other than 100%. Please adjust your DPI settings and try again. The program will terminate.");
                Application.Exit();
            }
            return Screen.GetBounds(System.Drawing.Point.Empty);
        }
    }
}
