using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using NumpyDotNet;
using System.Drawing.Drawing2D;

namespace MinesweeperSolver
{

    public partial class Form1 : Form
    { 
        Opaque oq;
        public Form1()
        {
            InitializeComponent();
        }
        private Rectangle ScreenBounds()
        {
            return Screen.GetBounds(System.Drawing.Point.Empty);
        }
        void SnipScreen()
        {
            Cursor.Current = System.Windows.Forms.Cursors.Cross;
            Rectangle bounds = ScreenBounds();
            FillScreen();
            oq = new Opaque();
            oq.Show();
            oq.Location = new Point(bounds.Width / 2 - (oq.Size.Width / 2), 0);
            this.BackColor = Color.Black;
            this.Opacity = 0.2;
            button3.Visible = button2.Visible = button1.Visible = false;
            start = true;
        }
        Rectangle regionRectangle;
        private void DoLogic(Rectangle region)
        {
            DoLogic(CaptureScreen(region));
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (start && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                start = false;
                oq.Hide();
                oq.Dispose();
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
        private Bitmap CaptureScreen(Rectangle snipRectangle)
        {

            Rectangle bounds = ScreenBounds();
            Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(System.Drawing.Point.Empty, System.Drawing.Point.Empty, bounds.Size);
            }
            if (snipRectangle.Width == 0 || snipRectangle.Height == 0) return null;

            bitmap = bitmap.Clone(snipRectangle, bitmap.PixelFormat);
            return bitmap;
        }
        private void FillScreen()
        {
            Rectangle bounds = ScreenBounds();
            this.Location = new System.Drawing.Point(0, 0);
            this.Size = new Size(bounds.Width, bounds.Height);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
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
            counter++;
        }
        private void SaveOpenImage(Image<Gray, Byte> image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            counter++;
        }
        private void SaveOpenImage(Image<Bgr, float> image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + filename);
            counter++;
        }
        private void SaveOpenImage(Image<Bgr,Byte> image, string filename)
        {
            image.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\"+filename);
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\"+filename);
            counter++;
        }
        int counter = 0;
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
            if (drawGrid) this.CreateGraphics().Clear(Color.White);
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
                    if (numberMap[_color] != 8 && pixels >= 10 || pixels > 80 )
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
                }   
                if ((number == 7 || number == -1) && whitePixels >= 1 && ImageColor(__bitmap, Color.FromArgb(0,0,0), 10) > 10)
                {
                    number = -3; //BOMB
                }
                if (drawGrid)
                {
                    this.CreateGraphics().DrawRectangle(new Pen(new SolidBrush(Color.Red)), selectX + rect.X, selectY + rect.Y, rect.Width, rect.Height);
                    this.CreateGraphics().DrawString(GetChar(number).ToString(), new Font("Arial", 8), new SolidBrush(Color.Navy), selectX + rect.X + 1, selectY + rect.Y);
                }
                Matrix[row, col] = number;
                lastY = rect.Y;
                col++;
            }
            return Matrix;

        }
        private void DoLogic(Bitmap _bitmap)
        {
            if (_bitmap == null) return;
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
                    /* ---------------------------------------------- */

                    /* Find contours */
                    /* ---------------------------------------------- */
                    Mat hierarchy = new Mat();
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    CvInvoke.FindContours(gray_img, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Ccomp, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
                    /* ---------------------------------------------- */

                    /* Filter rectangles */
                    /* ---------------------------------------------- */
                    List<Rectangle> _rect = new List<Rectangle>();
                    for (int i = 0; i < contours.Size; i++)
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                        if (rect.Width > 11 && rect.Height > 11 && rect.Height < 20 && rect.Width < 20)
                            _rect.Add(rect);
                    }
                    /* ---------------------------------------------- */
                    GetMatrix(_rect, original, true);
                }
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
            Form1_PreviewKeyDown(sender, null);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            this.CreateGraphics().Clear(Color.White);
            if (regionRectangle != null && regionRectangle.Width > 0 &regionRectangle.Height >0)
                DoLogic(regionRectangle);
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            this.CreateGraphics().Clear(Color.White);
        }
    }
}
