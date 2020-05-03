using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
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
        /* Import user32.dll to simulate mouse clicks */
        /* ------------------------------------------ */
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;
        /* ------------------------------------------ */

        public Form1()
        {
            InitializeComponent();
        }
        void SnipScreen()
        {
            Cursor.Current = System.Windows.Forms.Cursors.Cross;
            
            FillScreen();
            ShowMessage("Drag the mouse to select the interactive area of the game");
            this.BackColor = Color.Black;
            this.Opacity = 0.3;
            button3.Visible = button2.Visible = button1.Visible = false;
            start = true;
        }
        Rectangle regionRectangle;
        Point startcords;
        private void restartgame()
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
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (start && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                start = false;
                HideMessage();
                this.Opacity = 1;
                this.BackColor = Color.White;
                button3.Visible = button2.Visible = button1.Visible = true;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                DoLogic(regionRectangle);
            }
        }

        int selectX;
        int selectY;
        bool start = false;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (start && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Cursor.Current = System.Windows.Forms.Cursors.Cross;
                selectX = e.X;
                selectY = e.Y;
            }
        }
        Point lastPos = new Point(0,0);
        private void RightClick(uint X, uint Y)
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, X, Y, 0, 0);
            Thread.Sleep(1);
        }
        private void LeftClick(uint X, uint Y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, X, Y, 0, 0);
            Thread.Sleep(1);
        }
        private void HideMessage()
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
        private void ShowMessage(string text)
        {
            HideMessage();
            this.Invoke((MethodInvoker)delegate {

                this.CreateGraphics().Clear(Color.White);
                oq = new Opaque(text);
                oq.Show();
                oq.TopMost = true;
            });
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (start && e.Button == System.Windows.Forms.MouseButtons.Left && e.X - selectX > 0 && e.Y - selectY > 0)
            {
                Cursor.Current = System.Windows.Forms.Cursors.Cross;
                using (System.Drawing.SolidBrush brush = new System.Drawing.SolidBrush(System.Drawing.Color.White))
                {
                    using (Graphics g = this.CreateGraphics())
                    {
                        g.Clear(Color.Black);
                        regionRectangle = new Rectangle(selectX, selectY, e.X - selectX, e.Y - selectY);
                        g.FillRectangle(brush, regionRectangle);
                    }
                }
            }
        }
        private Image<Bgr,byte> CaptureScreenImg(Rectangle snipRectangle)
        {
            this.Invoke((MethodInvoker)delegate
            {
                this.Opacity = 0;
            });
            Image<Bgr, byte> _return = new Image<Bgr, byte>(CaptureScreen(snipRectangle));
            this.Invoke((MethodInvoker)delegate
            {
                this.Opacity = 1;
            });
            return _return;
            
        }
        private Bitmap CaptureScreen(Rectangle snipRectangle)
        {

            Rectangle bounds = Helper.ScreenBounds();
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
            }
            if (snipRectangle.Width == 0 || snipRectangle.Height == 0) return bitmap;

            bitmap = bitmap.Clone(snipRectangle, bitmap.PixelFormat);
            return bitmap;
        }
        private void FillScreen()
        {
            Rectangle bounds = Helper.ScreenBounds();
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new Size(bounds.Width, bounds.Height);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            if (MessageBox.Show("Would you like to enable auto-clicking for detections?", "MinesweeperSolver", MessageBoxButtons.YesNo) == DialogResult.Yes) autoMouse.Checked = true;
            FillScreen();
            numberMap.Add(Color.FromArgb(255, 0, 0, 255), 1);
            numberMap.Add(Color.FromArgb(255, 0, 128, 0), 2);
            numberMap.Add(Color.FromArgb(255, 255, 0, 0), 3);
            numberMap.Add(Color.FromArgb(255, 0, 0, 123), 4);
            numberMap.Add(Color.FromArgb(255, 129, 1, 2), 5);
            numberMap.Add(Color.FromArgb(255, 0, 128, 129), 6);
            numberMap.Add(Color.FromArgb(255, 0, 0, 0), 7);
            numberMap.Add(Color.FromArgb(255, 123,123,123), 8);
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.MouseMove += Form1_MouseMove;
            //Form1_PreviewKeyDown(null, null);
        }
        private char GetChar(int number)
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
        /*bool HasParent(Rectangle rect, List<Rectangle> rectangles)
        {
            foreach (Rectangle rectangle in rectangles)
            {
                if ((rect.X >= rectangle.X && rect.X <= rectangle.X + rectangle.Width) && (rect.Y >= rectangle.Y && rect.Y <= rectangle.Y + rectangle.Height))
                    return true;
            }
            return false;
        }*/

        private void SaveOpenImage(Bitmap image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            
        }
        private void SaveOpenImage(Image<Gray, Byte> image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            
        }
        private void SaveOpenImage(Image<Bgr, float> image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            
        }
        private void SaveOpenImage(Image<Bgr,Byte> image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\"+filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\"+filename);
            
        }
        private Image<Bgr, byte> ToWhite(Image<Bgr, byte> rgbimage, Bgr sourceColor)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor, sourceColor);
            var mat = ret.Mat;
            mat.SetTo(new MCvScalar(255,255,255), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Bgr, byte> ToPink(Image<Bgr, byte> rgbimage, Bgr sourceColor)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor, sourceColor);
            var mat = ret.Mat;
            mat.SetTo(new MCvScalar(168, 65, 255), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Bgr, byte> ToPink(Image<Bgr, byte> rgbimage, Bgr sourceColor, Bgr targetColor)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor,targetColor);
            var mat = ret.Mat;
            mat.SetTo(new MCvScalar(168, 65, 255), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Gray,byte> GetGrayImage(Image<Bgr, byte> rgbimage, Bgr sourceColor, Bgr targetColor)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = ret.InRange(sourceColor, targetColor);
            return image;
        }
        private List<Rectangle> ApproximateRectangles(List<Rectangle> rectangles, out int row, out int col)
        {
            int lastIndex = 0;
            row = 0;
            col = 0;
            rectangles = rectangles.OrderBy(x => x.X).OrderBy(x => x.Y).ToList();
            for (int i = 0; i < rectangles.Count(); i++)
            {
                if (Math.Abs(rectangles[i].Y - rectangles[lastIndex].Y) < rectangles[lastIndex].Height / 2) { Rectangle oldRect = rectangles[i]; oldRect.Y = rectangles[lastIndex].Y; rectangles[i] = oldRect; }
                else { row++;  col = 0; }
                col++;
                lastIndex = i;
            }
            if (col > 0) row++;
            return rectangles.OrderBy(x => x.X).OrderBy(x => x.Y).ToList();
        }
        private int[,] GetMatrix(List<Rectangle> cells, Image<Bgr, Byte> original, bool drawGrid = false)
        {
            int mrow, mcol;
            cells = ApproximateRectangles(cells, out mrow, out mcol);
            int lastY = -1;
            int number = 0;
            int row = 0;
            int col = 0;
            Bitmap __bitmap;
            int[,] Matrix = new int[mrow, mcol];
            Dictionary<string, int> cellsIndex = new Dictionary<string, int>();
            if(!autoMouse.Checked)  this.CreateGraphics().Clear(Color.White);
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
                    int pixels = ImageColor(__bitmap, _color, 8);
                    if (numberMap[_color] != 8 && pixels >= 10 || pixels > 80)
                    {
                        number = numberMap[_color];
                        break;
                    }
                }
                if (number == 3 && ImageColor(__bitmap, Color.FromArgb(255, 0, 0, 0), 5) > 10)
                {
                    number = -1; //FLAGGED
                }
                else if ((number == 0 || number == 8) && whitePixels >= 25)
                {
                    number = -2; //UNOPENED
                    cellsIndex.Add(col + "_" + row, i);
                }
                else if ((number == 7 || number == -1) && whitePixels >= 1 && ImageColor(__bitmap, Color.FromArgb(0, 0, 0), 10) > 10)
                {
                    number = -3; //BOMB
                    if (autoMouse.Checked)
                    {
                        restartgame();
                    }
                    else
                    {
                        this.CreateGraphics().Clear(Color.White);
                        ShowMessage("Game lost")
                    }
                    ;

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
            if (!solver.Solve())
            {
                
                if (autoMouse.Checked)
                {
                    restartgame();
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

            List<Panel> flags = new List<Panel>();
            flags = solver.GetFlags();

            List<Panel> will_reveal = new List<Panel>();
            will_reveal = solver.GetRevealed();
            foreach (var reveal in will_reveal)
            {
                Rectangle rect = cells[cellsIndex[reveal.X + "_" + reveal.Y]];
                if (autoMouse.Checked)
                {
                    Cursor.Position = new Point(selectX + rect.X + (rect.Width / 2), selectY + rect.Y + (rect.Height / 2));
                    LeftClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
                }
                else
                {
                    this.CreateGraphics().DrawRectangle(new Pen(new SolidBrush(Color.Blue)), selectX + rect.X, selectY + rect.Y, rect.Width, rect.Height);

                }
            }

            foreach (var flag in flags)
            {
                Rectangle rect = cells[cellsIndex[flag.X + "_" + flag.Y]];
                
                if (autoMouse.Checked)
                {
                    Cursor.Position = new Point(selectX + rect.X + (rect.Width / 2), selectY + rect.Y + (rect.Height / 2));
                    RightClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
                }
                else
                {
                    this.CreateGraphics().DrawRectangle(new Pen(new SolidBrush(Color.Red)), selectX + rect.X, selectY + rect.Y, rect.Width, rect.Height);
                }
            }
            return Matrix;

        }
        private void DoLogic(Bitmap _bitmap)
        {
            if (_bitmap == null) return;
            List<Rectangle> _rect = new List<Rectangle>();
            using (Image<Bgr, Byte> original = new Image<Bgr, byte>(_bitmap))
            {

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

                    /*int ycrop = 45;
                    gray_img.ROI = new Rectangle(0, ycrop, gray_img.Width, gray_img.Height - ycrop);
                    gray_img = gray_img.Clone();*/

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
                        if (rect.Width > 11 && rect.Height > 11 && rect.Height < 20 && rect.Width < 20)
                        {
                            //original.Draw(rect, new Bgr(Color.Pink), 2);
                            _rect.Add(rect);
                        }
                    }
                    //SaveOpenImage(original.Bitmap, "bitmapimg.png");
                    /* ---------------------------------------------- */
                }
            }
            try
            {
                if (worker != null)
                    worker.Abort();
            }
            catch { }
            if (worker == null || worker.ThreadState != System.Threading.ThreadState.Running)
            {
                int _wait = (int)refreshFrequency.Value;
                worker = new Thread((ThreadStart)delegate
                {
                    string lastAverage = "";
                    while (true)
                    {
                        using (Image<Bgr, byte> _captured = CaptureScreenImg(regionRectangle))
                        {
                            if (lastAverage != _captured.GetAverage().ToString())
                            {
                                lastAverage = _captured.GetAverage().ToString();
                                GetMatrix(_rect, _captured, false);
                            }
                        }
                        Thread.Sleep(_wait);
                    }
                });
                worker.Start();
            } 
        }

        Dictionary<Color, int> numberMap = new Dictionary<Color, int>();
        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e == null || e.Control && e.KeyCode == Keys.T)
            {
                SnipScreen();
            }
        }
        int ImageColor(Bitmap image, Color _color, int thres = 10)
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
        private void Button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (worker != null)
                    worker.Abort();

                var img = new Image<Bgr, byte>(CaptureScreen(Helper.ScreenBounds()));
                var filtrdimg = img.InRange(new Bgr(189, 189, 189), new Bgr(205, 205, 205));

                /*python code  */
                //img_contoured = img.copy()
                //cnts = cv2.findContours(filtrdimg, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)[-2]
                //cv2.drawContours(img_contoured, cnts, -1, (0,255,0), 3)
                //show(img_contoured)

                VectorOfVectorOfPoint cnts = new VectorOfVectorOfPoint();
                CvInvoke.FindContours(filtrdimg, cnts, null, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
                var asdx = CvInvoke.ContourArea(cnts[0]);
                var biggstcontour = (from u in cnts.ToArrayOfArray()
                                     orderby CvInvoke.ContourArea(new VectorOfPoint(u)) descending
                                     select u).FirstOrDefault();

                img.Draw(biggstcontour, new Bgr(0, 255, 0), 3);

                regionRectangle = CvInvoke.BoundingRectangle(new VectorOfPoint(biggstcontour));
                selectX = regionRectangle.X;
                selectY = regionRectangle.Y;


                /*************Storing coordinates of the Yellow Smile button**************************/
                /*Python Code*/
                //img = cv2.imread('mines3.png')
                //template = cv2.imread('startsmil1.png')
                //h,w = (13, 12)
                //res = cv2.matchTemplate(img, template, cv2.TM_CCORR)
                //min_val, max_val, min_loc, max_loc = cv2.minMaxLoc(res)
                //top_left = max_loc
                //bottom_right = (top_left[0] + w, top_left[1] + h)
                //cv2.rectangle(img, top_left, bottom_right, (0, 0, 255), 2)

                var imgcrpd = new Image<Bgr, byte>(CaptureScreen(regionRectangle));

                var template = new Image<Bgr, Byte>("startsmile.png");
                var res = imgcrpd.MatchTemplate(template, Emgu.CV.CvEnum.TemplateMatchingType.Ccorr);

                res.MinMax(out double[] min_val, out double[] max_val, out Point[] min_loc, out Point[] max_loc);

                startcords = new Point(selectX + max_loc[0].X + 6, selectY + max_loc[0].Y + 6);             
                                                           
                if(autoMouse.Checked ) restartgame();
                DoLogic(regionRectangle);
            }
            catch
            {
                MessageBox.Show("Error happened In Capturing , Please adjust the game");
            }
            //Form1_PreviewKeyDown(sender, null);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            this.CreateGraphics().Clear(Color.White);
            if (regionRectangle != null && regionRectangle.Width > 0 &regionRectangle.Height >0)
                DoLogic(regionRectangle);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (worker != null)
                worker.Abort();
            }
            catch { }
            if (oq != null && !oq.IsDisposed)
            {
                oq.Hide();
                oq.Dispose();
            }
            this.CreateGraphics().Clear(Color.White);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(oq != null) oq.Dispose();
            worker.Abort();
            System.Windows.Forms.Application.Exit();
        }
    }
}
