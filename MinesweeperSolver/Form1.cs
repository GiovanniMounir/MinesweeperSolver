/* Copyright 2020. Developed by Giovanni Mounir, Paul Ragheb, Beshoy Abd Elsayed, Ahmed Atef, Ahmed Anter

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

namespace MinesweeperSolver
{

    public partial class Form1 : Form
    { 
        Opaque oq;
        Thread worker;
        Rectangle regionRectangle; //borders of minesweeper window
        Point startcords; // coordinates of theyellow start button
        int selectX;
        int selectY;
        List<Rectangle> blue_overlays = new List<Rectangle>();
        List<Rectangle> red_overlays = new List<Rectangle>();
        Dictionary<Color, int> numberMap = new Dictionary<Color, int>();
        /* ------------------------------------------ */
/* Import user32.dll to simulate mouse clicks */
[DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        /* ------------------------------------------ */

        public Form1()
        {
            InitializeComponent();
            // Register HotKey
            // Set an unique id to your Hotkey, it will be used to
            // identify which hotkey was pressed in code to execute something
            int UniqueHotkeyId = 1;
            // Set the Hotkey triggerer the F1 key 
            // Expected an integer value for F1
            int HotKeyCode = (int)Keys.F1;
            // Register the "F1" hotkey
            Boolean F1Registered = RegisterHotKey(
                this.Handle, UniqueHotkeyId, 0x0000, HotKeyCode
            );

            //Verify if the hotkey was succesfully registered, if not, show message in the console
            if (!F1Registered)
            {
                MessageBox.Show("There has been an error trying to register the F1 hotkey to exit auto-mouse mode. Please make sure that high frequency values are used in order to exit auto-mouse easier.");
            }
        }

        protected override void WndProc(ref Message m)
        {
            //Catch when a HotKey is pressed !
            if (m.Msg == 0x0312)
            {
                int id = m.WParam.ToInt32();
                // MessageBox.Show(string.Format("Hotkey #{0} pressed", id));

                if (id == 1)
                {
                    autoMouse.Checked = false;
                }
            }

            base.WndProc(ref m);
        }

        private void RestartGame() //Press on yellow start/smiley button
        {
            Cursor.Position = new Point(startcords.X, startcords.Y);
            LeftClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
            int xrandom =selectX + regionRectangle.Width / 2;
            int yrandom = selectY + regionRectangle.Height / 2;
            Cursor.Position = new Point(xrandom, yrandom);
            LeftClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
        }
        private void DoLogic(Rectangle region)
        {
            DoLogic(CaptureScreen(region));
        }
        private void RightClick(uint X, uint Y) //Simulate right click, used by auto-mouse
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0); 
            Thread.Sleep(1);
        }
        private void LeftClick(uint X, uint Y)  //Simulate left click, used by auto-mouse
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            Thread.Sleep(1);
        }

        
        private void HideMessage() //Hide shown message on top to user 
        {
            if (oq != null && !oq.IsDisposed)
            {
                this.Invoke((MethodInvoker)delegate
                {
                    oq.Hide();
                    oq.Dispose();
                });
            }
        }
        private void ClearGraphics() //Clear overlays (blue/red rectangles by the application)
        {
            this.CreateGraphics().Clear(Color.White);
        }
        private void ShowMessage(string text) //Show message on top to user 
        {
            HideMessage();
            this.Invoke((MethodInvoker)delegate {
                oq = new Opaque(text);
                oq.Show();
                oq.TopMost = true;
            });
        }
        private Bitmap CaptureScreen(Rectangle snipRectangle) //Take a screenshot of the screen within the "snipRectangle" (if exists) bounds
        {

            Rectangle bounds = Helper.ScreenBounds();
            Bitmap bitmap;
            /* Check if snipRectangle is valid. If not, capture whole screen */
            if (snipRectangle.Width == 0 || snipRectangle.Height == 0)
                bitmap = new Bitmap(bounds.Width, bounds.Height);
            else
                bitmap = new Bitmap(snipRectangle.Width, snipRectangle.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                ClearGraphics();
                if (snipRectangle.Width == 0 || snipRectangle.Height == 0)
                    g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
                else
                    g.CopyFromScreen(snipRectangle.Left, snipRectangle.Top, 0, 0, bitmap.Size, CopyPixelOperation.SourceCopy);
                DrawOverlays();
            }
            return bitmap;
        }
        private Image<Bgr, byte> CaptureScreenImg(Rectangle snipRectangle) //Same as CaptureScreen, but returns Image<Bgr, byte> instead of a Bitmap
        {
            Image<Bgr, byte> _return = new Image<Bgr, byte>(CaptureScreen(snipRectangle));
            return _return;
        }
        
        private void FillScreen() //Fit the application's overlay to the screen bounds
        {
            Rectangle bounds = Helper.ScreenBounds();
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new Size(bounds.Width, bounds.Height);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            ShowMessage("Open the game and click on Capture to start. Click on Capture again if you move the game window.");
            if (MessageBox.Show("Would you like to enable auto-clicking for detections?", "MinesweeperSolver", MessageBoxButtons.YesNo) == DialogResult.Yes) autoMouse.Checked = true;
            FillScreen();
            /* numberMap is used to link colors in cells with numbers */
            numberMap.Add(Color.FromArgb(255, 0, 0, 255), 1);
            numberMap.Add(Color.FromArgb(255, 0, 128, 0), 2);
            numberMap.Add(Color.FromArgb(255, 255, 0, 0), 3);
            numberMap.Add(Color.FromArgb(255, 0, 0, 123), 4);
            numberMap.Add(Color.FromArgb(255, 129, 1, 2), 5);
            numberMap.Add(Color.FromArgb(255, 0, 128, 129), 6);
            numberMap.Add(Color.FromArgb(255, 0, 0, 0), 7);
            numberMap.Add(Color.FromArgb(255, 123,123,123), 8);
            
           
        }

        private char GetChar(int number) //Translate cells code to characters
        {
            switch (number)
            {
                case -3:
                    return 'B';
                case -2:
                    return 'U';
                case -1: return 'F';
                default:
                    return number.ToString()[0];
            }
        }
        private Image<Bgr, byte> ToPink(Image<Bgr, byte> rgbimage, Bgr sourceColor) //converts "sourceColor" in "rgbimage" to pink (168,65,255)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor, sourceColor);
            var mat = ret.Mat;
            mat.SetTo(new MCvScalar(168, 65, 255), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Bgr, byte> ToPink(Image<Bgr, byte> rgbimage, Bgr sourceColor, Bgr targetColor)  //converts colors between "sourceColor" and "targetColor" in "rgbimage" to pink (168,65,255)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor,targetColor);
            var mat = ret.Mat;
            mat.SetTo(new MCvScalar(168, 65, 255), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Gray,byte> GetGrayImage(Image<Bgr, byte> rgbimage, Bgr sourceColor, Bgr targetColor) //returns gray image given rgb image
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor, targetColor);
            return image;
        }
        private List<Rectangle> ApproximateRectangles(List<Rectangle> rectangles, out int row, out int col)
        {
            //Because the rectangles retreived in the "rectangles" list is unordered, we order them here according to their X and Y positions for easier matrix construction
            int lastIndex = 0;
            row = 0;
            col = 0;
            rectangles = rectangles.OrderBy(x => x.X).OrderBy(x => x.Y).ToList();
            for (int i = 0; i < rectangles.Count(); i++)
            {
                if (Math.Abs(rectangles[i].Y - rectangles[lastIndex].Y) < rectangles[lastIndex].Height / 2) //Proceed only when a significant change in Y is detected (this means that we have moved into a new row)
                {
                    Rectangle oldRect = rectangles[i];
                    oldRect.Y = rectangles[lastIndex].Y;
                    rectangles[i] = oldRect;
                }
                else { row++;  col = 0; }
                col++;
                lastIndex = i;
            }
            if (col > 0) row++;
            return rectangles.OrderBy(x => x.X).OrderBy(x => x.Y).ToList();
        }
        private void DrawOverlays() //Draws the red/blue overlays in the overlay lists
        {
            ClearGraphics(); //Clear any existent overlays before drawing
            foreach (Rectangle rect in red_overlays)
            {
                this.CreateGraphics().DrawRectangle(new Pen(new SolidBrush(Color.Red)), rect);
            }
            foreach (Rectangle rect in blue_overlays)
            {
                this.CreateGraphics().DrawRectangle(new Pen(new SolidBrush(Color.Blue)), rect);
            }
        }
        private void ClearOverlays() //Clear blue and red overlays from the overlay list; does not actually clear them from the graphics/form
        {
            blue_overlays.Clear();
            red_overlays.Clear();
        }
        private int[,] GetMatrix(List<Rectangle> cells, Image<Bgr, Byte> original, int mrow, int mcol, bool drawGrid = false)
        {
            int lastY = -1;
            int number;
            int row = 0;
            int col = 0;
            Bitmap __bitmap;
            int[,] Matrix = new int[mrow, mcol]; //Create integer matrix of mrow x mcol
            Dictionary<string, int> cellsIndex = new Dictionary<string, int>();
            for (int i = 0; i < cells.Count(); i++)
            {
                Rectangle rect = cells[i];
                rect.X -= 1;
                if (lastY == -1) lastY = rect.Y;
                if (lastY != rect.Y) { row++; col = 0; }
                __bitmap = original.ToBitmap().Clone(new Rectangle(rect.X, rect.Y, rect.Width, rect.Height), original.ToBitmap().PixelFormat);
                number = 0; //BLANK
                int whitePixels = ImageColor(__bitmap, Color.FromArgb(255, 255, 255, 255));
                foreach (Color _color in numberMap.Keys)
                {
                    int pixels = ImageColor(__bitmap, _color, 8); //Check how many have that color (with error rate of -/+8 RGB values)
                    if (numberMap[_color] != 8 && pixels >= 10 || pixels > 80) //This is our color if there's more than 10 pixels. 8 is special because it uses 123,123,123 which is common throughout the cells. Mark as 8 only if 123,123,123 exists in 80 or more pixels
                    {
                        number = numberMap[_color]; //Retrieve number from numberMap according to color
                        break;
                    }
                }
                if (number == 3 && ImageColor(__bitmap, Color.FromArgb(255, 0, 0, 0), 5) > 10) //3 and Flagged cells both have red colors, mark as Flagged if both red and black exist
                {
                    number = -1; //FLAGGED
                }
                else if ((number == 0 || number == 8) && whitePixels >= 25) //Mark as unopened if there are white pixels if number was detected to be 0 or 8. 0 is the default, and the color of 8 is common (border color), which is why we are using it in the IF condition.
                {
                    number = -2; //UNOPENED
                    cellsIndex.Add(col + "_" + row, i);
                }
                if ((number == 7 || number == -1) && whitePixels >= 1 && ImageColor(__bitmap, Color.FromArgb(0, 0, 0), 10) > 25 && ImageColor(__bitmap, Color.FromArgb(255, 0, 0), 10) > 10) //User clicked on a bomb: the color of 7 is black. A bomb color is black and has a few white pixels. The bomb the user clicks on has a red background
                {
                    if (autoRestart.Checked)
                    {
                        RestartGame();
                    }
                    else
                    {
                        ClearGraphics();
                        ShowMessage("Game lost");
                    }
                    return new int[,] { };
                }
                if (drawGrid && !autoMouse.Checked)
                {
                    this.CreateGraphics().DrawRectangle(new Pen(new SolidBrush(Color.Red)), selectX + rect.X, selectY + rect.Y, rect.Width, rect.Height);
                    this.CreateGraphics().DrawString(GetChar(number).ToString(), new Font("Arial", 8), new SolidBrush(Color.Navy), selectX + rect.X + 1, selectY + rect.Y);
                }
                Matrix[row, col] = number;
                lastY = rect.Y;
                col++;
            }
            GameSolver solver = new GameSolver(Matrix.GetLength(0), Matrix.GetLength(1), Matrix);
            if (!solver.Solve()) //If there is no 100% solution
            {
                
                if (autoRestart.Checked)
                {
                    RestartGame();
                }
                else
                {
                    ShowMessage("There is no solution");
                }
            }
            else
            {
                HideMessage();
            }
            ClearOverlays();
            foreach (var reveal in solver.GetRevealed()) //For each safe cells
            {
                Rectangle rect = cells[cellsIndex[reveal.X + "_" + reveal.Y]];
                if (autoMouse.Checked)
                {
                    //Move mouse to the cell and click
                    Cursor.Position = new Point(selectX + rect.X + (rect.Width / 2), selectY + rect.Y + (rect.Height / 2));
                    LeftClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
                }
                else
                {
                    blue_overlays.Add(new Rectangle(selectX + rect.X, selectY + rect.Y, rect.Width, rect.Height));

                }
            }

            foreach (var flag in solver.GetFlags()) //For each bomb (i.e should be flagged)
            {
                Rectangle rect = cells[cellsIndex[flag.X + "_" + flag.Y]];
                if (autoMouse.Checked)
                {
                    //Move mouse to cell and right-click
                    Cursor.Position = new Point(selectX + rect.X + (rect.Width / 2), selectY + rect.Y + (rect.Height / 2));
                    RightClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
                }
                else
                {
                    red_overlays.Add(new Rectangle(selectX + rect.X, selectY + rect.Y, rect.Width, rect.Height));
                }
            }
            DrawOverlays();
            return Matrix;

        }


       
        private void DoLogic(Bitmap _bitmap)  //This function takes an image of the window of the game and extracts cells from it. The extracted cells are then run through GetMatrix every time the milliseconds defined in ("refresh frequency") elapse through the "worker" thread
        {
            if (_bitmap == null) return;
            List<Rectangle> _rect = new List<Rectangle>(); //This list will be used to save the positions of the cells
            using (Image<Bgr, Byte> original = new Image<Bgr, byte>(_bitmap))
            {

                /* Convert potential borders based on their color to pink; this will help us extract the grid */ 
                using (Image<Bgr, Byte> img = ToPink(ToPink(ToPink(ToPink(ToPink(original, new Bgr(200, 200, 200), new Bgr(255, 255, 255)), new Bgr(123, 123, 123)), new Bgr(129, 129, 129), new Bgr(180, 180, 180)), new Bgr(255, 255, 254)), new Bgr(128, 128, 128)))
                {
                    Image<Gray, Byte> gray_img = GetGrayImage(img, new Bgr(168, 65, 255), new Bgr(168, 65, 255)); //Extract pink

                    /* Dilate/erode to remove noise/letters from grid */
                    /* ---------------------------------------------- */
                    Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(1, 3), new Point(-1, -1));
                    CvInvoke.Dilate(gray_img, gray_img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                    kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(1, 3), new Point(-1, -1));
                    CvInvoke.Erode(gray_img, gray_img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

                    kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 1), new Point(-1, -1));
                    CvInvoke.Dilate(gray_img, gray_img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                    kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 1), new Point(-1, -1));
                    CvInvoke.Erode(gray_img, gray_img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

                    kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 1), new Point(-1, -1));
                    CvInvoke.Dilate(gray_img, gray_img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                    kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 1), new Point(-1, -1));
                    CvInvoke.Erode(gray_img, gray_img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

                    

                    /* ---------------------------------------------- */

                    /* Find contours */
                    /* ---------------------------------------------- */
                    Mat hierarchy = new Mat();
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    CvInvoke.FindContours(gray_img, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Ccomp, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
                    /* ---------------------------------------------- */

                    /* Filter rectangles */
                    /* ---------------------------------------------- */
                    for (int i = 0; i < contours.Size; i++)
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                        if (rect.Width > 11 && rect.Height > 11 && rect.Height < 20 && rect.Width < 20) //We pick rectangles between 11x11 and 20x20, since the Minesweeper cells are usually between these dimensions
                        {
                            
                            _rect.Add(rect);
                        }
                    }
                    /* ---------------------------------------------- */
                }
            }
            try
            {
                if (worker != null)
                    worker.Abort(); //Stop any "worker" thread in the background as we will start a new process
            }
            catch { }
            _rect = ApproximateRectangles(_rect, out int mrow, out int mcol); //Order "cells" according to their X/Y positions. Also gives mrow and mcol, which indicate how many rows and columns exist
            if (worker == null || worker.ThreadState != System.Threading.ThreadState.Running)
            {
                int _wait = (int)refreshFrequency.Value; //Get user input; this is done outside the thread so that the user has to click "Capture" in order to prevent incorrect captures of user input while the user is filling the field
                worker = new Thread((ThreadStart)delegate
                {
                    while (true)
                    {
                        using (Image<Bgr, byte> _captured = CaptureScreenImg(regionRectangle)) //Capture the screen and wait "refresh frequency" between each capture
                        {
                            GetMatrix(_rect, _captured, mrow, mcol); //Process the matrix from the captured screen, given the rectangles locations and grid size
                        }
                        Thread.Sleep(_wait); //Wait between captures as per user input
                    }
                });
                worker.Start();
            } 
        }

        
        int ImageColor(Bitmap image, Color _color, int thres = 10) //Finds how many pixels contain the given color (with "thres" as acceptable deviation from RGB values)
        {
            Color temp;
            int count = 0;
            for (int i =0; i < image.Width; i++)
            {
                for (int j = 0; j< image.Height; j++)
                {
                    temp = image.GetPixel(i, j);
                    if ((Math.Abs(temp.R - _color.R) < thres) && (Math.Abs(temp.G - _color.G) < thres) && (Math.Abs(temp.B - _color.B) < thres)) count++;
                }
            }
            return count;
        }
        //OnClick of Capture Button
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (worker != null)
                    worker.Abort();

                using (var img = new Image<Bgr, byte>(CaptureScreen(Helper.ScreenBounds())))
                {
                    var filtrdimg = img.InRange(new Bgr(189, 189, 189), new Bgr(205, 205, 205)); //Threshold the image, the window we need is usually within these colors

                    VectorOfVectorOfPoint cnts = new VectorOfVectorOfPoint();
                    CvInvoke.FindContours(filtrdimg, cnts, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple); //Get contours in the thresholded image
                    var biggstcontour = (from u in cnts.ToArrayOfArray()
                                         orderby CvInvoke.ContourArea(new VectorOfPoint(u)) descending
                                         select u).FirstOrDefault(); //Get the contour with the biggest area

                    /* Store the location of the biggest contour */
                    regionRectangle = CvInvoke.BoundingRectangle(new VectorOfPoint(biggstcontour));
                    selectX = regionRectangle.X;
                    selectY = regionRectangle.Y;

                    using (var imgcrpd = new Image<Bgr, byte>(CaptureScreen(regionRectangle)))
                    {
                        using (var template = new Image<Bgr, Byte>("startsmile.png")) //startmsile.png is the smiley that appears on game start
                        {
                            var res = imgcrpd.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.Ccorr); //Match the smiley to find the contour of the game window
                            res.MinMax(out double[] min_val, out double[] max_val, out Point[] min_loc, out Point[] max_loc);

                            startcords = new Point(selectX + max_loc[0].X + 7, selectY + max_loc[0].Y + 7); //Get the coordinates of the smiley button, with 7px extra to account for middle of the button

                            DoLogic(regionRectangle);
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("The game window was not found. This programs only supports the Windows XP Minesweeper application or http://minesweeperonline.com/");
            }
            
        }

        //OnClick x button , abort thread and exit application
        private void Button4_Click(object sender, EventArgs e)
        {
            if(oq != null) oq.Dispose();
            try
            {
                worker.Abort();
            }
            catch
            {

            }
            System.Windows.Forms.Application.Exit();
        }

        private void AutoMouse_CheckedChanged(object sender, EventArgs e) //If the user clicks on auto-mouse, clear the overlays (i.e while enabling automouse, so that the overlays disappear)
        {
            ClearGraphics();
        }
        /* Informative */
        private void RestoreLabel2(object sender, EventArgs e)
        {
            label2.Text = "Press F1 to exit auto mouse";
        }

        private void AutoRestart_MouseHover(object sender, EventArgs e)
        {
            label2.Text = "The game will auto-restart when lost or when no 100% solution";
        }

        private void AutoMouse_MouseHover(object sender, EventArgs e)
        {
            label2.Text = "Automatically flag mines and open cells";
        }

        private void RefreshFrequency_Enter(object sender, EventArgs e)
        {
            label2.Text = "Indicates how often in milliseconds the program checks the screen";

        }
        private void Button1_MouseHover(object sender, EventArgs e)
        {
            label2.Text = "Capture the game window and check for updates";
        }

        private void Button4_MouseHover(object sender, EventArgs e)
        {
            label2.Text = "Close the application";

        }
    }
}
