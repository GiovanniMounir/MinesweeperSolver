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
    { //save resulting image to desktop
      /*

  // (2) Morph-op to remove noise
  Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Ellipse, new Size(11, 11), new Point(-1, -1));
  CvInvoke.MorphologyEx(img, img, Emgu.CV.CvEnum.MorphOp.Close, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
  */
      // (3) Find the max-area contour

        /*
		Mat hierarchy = new Mat();
		VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
		CvInvoke.FindContours(img, contours, hierarchy, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);
		Dictionary<int, List<Rectangle>> _rect = new Dictionary<int, List<Rectangle>>();
		for (int i =0; i <contours.Size; i++)
		{
			Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
			if (rect.Width > 5 && rect.Width == rect.Height)
			{
				original.Draw(rect, new Bgr(Color.Red), 2);
				if (!_rect.ContainsKey(rect.Width))
				{
					_rect.Add(rect.Width, new List<Rectangle>());
				}
				_rect[rect.Width].Add(rect);
			}
		}
		int maxCount = 0;
		int keyIndex = -1;
		foreach (int key in _rect.Keys)
		{
			if (_rect[key].Count > maxCount)
			{
				maxCount = _rect[key].Count;
				keyIndex = key;
			}
		}

		if (keyIndex != -1)
		{
			foreach (Rectangle rect in _rect[keyIndex])
			{
				//original.Draw(rect, new Bgr(Color.Red), 2);
			}
		}*/
        public static double PI = 3.14159265359;
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
            Rectangle bounds = ScreenBounds();
            FillScreen();
            oq = new Opaque();
            oq.Show();
            oq.Location = new Point(bounds.Width / 2 - (oq.Size.Width / 2), 0);
            this.BackColor = Color.Black;
            this.Opacity = 0.1;
            button1.Visible = false;
            button2.Visible = false;
        }
        Rectangle regionRectangle;
        private void DoLogic(Rectangle region)
        {
            DoLogic(CaptureScreen(region));
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                start = false;
                oq.Hide();
                oq.Dispose();
                this.Opacity = 1;
                this.BackColor = Color.White;
                button1.Visible = true;
                button2.Visible = true;
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
                DoLogic(regionRectangle);
            }
        }

        int selectX;
        int selectY;
        bool start = false;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                selectX = e.X;
                selectY = e.Y;
                start = true;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            Cursor.Current = System.Windows.Forms.Cursors.Cross;
            if (start && e.Button == System.Windows.Forms.MouseButtons.Left && e.X - selectX > 0 && e.Y - selectY > 0)
            {
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
            numberMap.Add(Color.FromArgb(255, 0, 123, 0), 2);
            numberMap.Add(Color.FromArgb(255, 255, 0, 0), 3);
            numberMap.Add(Color.FromArgb(255, 0, 0, 123), 4);
            numberMap.Add(Color.FromArgb(255, 129, 1, 2), 5);
            numberMap.Add(Color.FromArgb(255, 0, 128, 129), 6);
            numberMap.Add(Color.FromArgb(255, 0, 0, 0), 7);
            numberMap.Add(Color.FromArgb(255, 128, 128, 128), 8);
            this.MouseDown += Form1_MouseDown;
            this.MouseUp += Form1_MouseUp;
            this.MouseMove += Form1_MouseMove;
            //Form1_PreviewKeyDown(null, null);
        }
        private char GetChar(int number)
        {
            switch (number)
            {
                case -2:
                    return 'U';
                case -1: return 'F';
                default:
                    return number.ToString()[0];
            }
        }
        bool HasParent(Rectangle rect, List<Rectangle> rectangles)
        {
            foreach (Rectangle rectangle in rectangles)
            {
                if ((rect.X >= rectangle.X && rect.X <= rectangle.X + rectangle.Width) && (rect.Y >= rectangle.Y && rect.Y <= rectangle.Y + rectangle.Height))
                {
                    return true;
                }
            }
            return false;
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
            var image = rgbimage.InRange(sourceColor, sourceColor);
            var mat = rgbimage.Mat;
            mat.SetTo(new MCvScalar(255,255,255), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Bgr, byte> ToBlack(Image<Bgr, byte> rgbimage, Bgr sourceColor)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = rgbimage.InRange(sourceColor, sourceColor);
            var mat = rgbimage.Mat;
            mat.SetTo(new MCvScalar(0, 0, 0), image);
            mat.CopyTo(ret);
            return ret;
        }
        private Image<Bgr, byte> ToBlack(Image<Bgr, byte> rgbimage, Bgr sourceColor, Bgr targetColor)
        {
            Image<Bgr, byte> ret = rgbimage.Clone();
            var image = rgbimage.InRange(sourceColor,targetColor);
            var mat = rgbimage.Mat;
            mat.SetTo(new MCvScalar(0,0,0), image);
            mat.CopyTo(ret);
            return ret;
        }
        private void DoLogic(Bitmap _bitmap)
        {
            if (_bitmap == null) return;
            //_bitmap = new Bitmap(@"C:\Users\PC\Downloads\mines\Untitled.png");
            using (Image<Bgr, Byte> original = new Image<Bgr, byte>(_bitmap))
            {

                using (Image<Bgr, Byte> img = ToBlack(ToBlack(ToBlack(original, new Bgr(200, 200, 200), new Bgr(255, 255, 255)), new Bgr(128, 128, 128)), new Bgr(129, 129, 129), new Bgr(180, 180, 180)))
                {
                    // (1) Convert to gray, and threshold
                    //Emgu.CV.CvInvoke.CvtColor(img, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                    //Emgu.CV.CvInvoke.AdaptiveThreshold(img, img, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.BinaryInv, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
                    //CvInvoke.GaussianBlur(img, img, new Size(5, 5), 0);
                    CvInvoke.CvtColor(img, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                     int n = 2;

                     Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(n, n), new Point(-1, -1));
                     CvInvoke.Erode(img, img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                     kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(n, n), new Point(-1, -1));
                     CvInvoke.Dilate(img, img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                     
                    //Image<Bgr, float> sobel = img.Sobel(0, 1, 3).Add(img.Sobel(1, 0, 3)).AbsDiff(new Bgr(0,0,0));

                    SaveOpenImage(img, "imgor.png");
                    return;
                    CvInvoke.Canny(img, img, 50, 150);

                    List<Rectangle> rectangles = new List<Rectangle>();
                    Mat hierarchy = new Mat();
                    VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                    CvInvoke.FindContours(img, contours, hierarchy, Emgu.CV.CvEnum.RetrType.Ccomp, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxNone);
                    Dictionary<string, int> _rect = new Dictionary<string, int>();
                    for (int i = 0; i < contours.Size; i++)
                    {
                        Rectangle rect = CvInvoke.BoundingRectangle(contours[i]);
                        original.Draw(rect, new Bgr(Color.Blue), 1);
                        if (rect.Width > 5 && rect.Width < 20 && rect.Height < 20)
                            rectangles.Add(rect);
                    }
                    List<Rectangle> drawnRectangles = new List<Rectangle>();
                    rectangles = rectangles.OrderByDescending(x => x.Height).OrderByDescending(x => x.Width).ToList();
                    foreach (Rectangle rect in rectangles)
                    {
                        {
                            if (!HasParent(rect, drawnRectangles))
                            {
                                original.Draw(rect, new Bgr(Color.Red), 1);

                                drawnRectangles.Add(rect);
                                if (!_rect.ContainsKey("y_" + rect.Y))
                                {
                                    _rect.Add("y_" + rect.Y, 0);
                                }
                                if (!_rect.ContainsKey("x_" + rect.X))
                                {
                                    _rect.Add("x_" + rect.X, 0);
                                }
                                _rect["x_" + rect.X]++;
                                _rect["y_" + rect.Y]++;

                            }
                        }
                    }
                    /*
                    int rectwidth = 0;
                    int rectheight = 0;
                    for (int i = 0; i < rectangles.Count; i++)
                    {
                        rectwidth += rectangles[i].Width;
                        rectheight += rectangles[i].Height;
                    }
                    if (rectangles.Count > 0)
                    {
                        rectwidth /= rectangles.Count;
                        rectheight /= rectangles.Count;
                        int maxCount_x = 0;
                        int maxCount_y = 0;
                        string maxkeyIndex_x = "";
                        string maxkeyIndex_y = "";
                        foreach (string key in _rect.Keys)
                        {
                            if (key.StartsWith("x_") && _rect[key] > maxCount_x)
                            {
                                maxCount_x = _rect[key];
                                maxkeyIndex_x = key;
                            }
                            else if (key.StartsWith("y_") && _rect[key] > maxCount_y)
                            {
                                maxCount_y = _rect[key];
                                maxkeyIndex_y = key;
                            }
                        }
                        if (maxCount_x * maxCount_y != rectangles.Count)
                        {
                            MessageBox.Show("The cells were not properly detected. Please adjust the snipping rectangle.");
                            //return;
                        }
                        rectangles = rectangles.OrderBy(x => x.X).OrderBy(x => x.Y).ToList();
                        int[,] Matrix = new int[maxCount_x, maxCount_y];
                        int number;
                        int _i = 0;
                        for (int x = 0; x < maxCount_x; x++)
                        {
                            for (int y = 0; y < maxCount_y; y++)
                            {
                                if (_i < rectangles.Count)
                                {
                                    Bitmap __bitmap = original.ToBitmap().Clone(new Rectangle(rectangles[_i].X - 3, rectangles[_i].Y, rectwidth + 3, rectheight), original.ToBitmap().PixelFormat);
                                    number = 0; //BLANK
                                    foreach (Color _color in numberMap.Keys)
                                    {
                                        if (ImageColor(__bitmap, _color, 8))
                                        {
                                            number = numberMap[_color];
                                            break;
                                        }
                                    }
                                    if (number == 0 && ImageColor(__bitmap, Color.FromArgb(255, 0, 0, 0), 10) && ImageColor(__bitmap, Color.FromArgb(255, 254, 0, 0), 10))
                                    {
                                        number = -1; //FLAGGED
                                    }
                                    else if (number == 0 && ImageColor(__bitmap, Color.FromArgb(255, 255, 255, 255), 30))
                                    {
                                        number = -2; //UNOPENED
                                    }
                                    visualizer.Draw(new Rectangle(rectangles[_i].X - 3, rectangles[_i].Y, rectwidth + 3, rectheight), new Bgr(0, 0, 255), 1);
                                    visualizer.Draw(GetChar(number).ToString(), new Point(rectangles[_i].X + 1, rectangles[_i].Y + 5), Emgu.CV.CvEnum.FontFace.HersheyPlain, 0.5, new Bgr(255, 255, 255));
                                    Matrix[x, y] = number;
                                    _i++;
                                }
                            }
                        }
                        visualizer.Save(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\visualizer.png");
                        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\visualizer.png");
                    }
                }
                    */
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
        bool ImageColor(Bitmap image, Color _color, int mincount = 8, int thres = 5)
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
            if (count >= mincount) return true;
            return false;/*
            for (int i = 0; i < image.Height * image.Width; ++i)
            {
                int row = i / image.Height;
                int col = i % image.Width;
                if (row % 2 != 0) col = image.Width - col - 1;
                
                if (count >= mincount) return true;
            }
            return false;*/

        }
        private void Button1_Click(object sender, EventArgs e)
        {
            Form1_PreviewKeyDown(sender, null);
        }
        private void Button2_Click(object sender, EventArgs e)
        {
            if (regionRectangle != null && regionRectangle.Width > 0 &regionRectangle.Height >0)
                DoLogic(regionRectangle);
        }
    }
}
