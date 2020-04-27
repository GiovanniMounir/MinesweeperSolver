using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using NumpyDotNet;

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
            numberMap.Add(Color.FromArgb(255, 0, 0, 255), 1);
            numberMap.Add(Color.FromArgb(255, 0, 123, 0), 2);
            numberMap.Add(Color.FromArgb(255, 255, 0, 0), 3);
            numberMap.Add(Color.FromArgb(255, 0, 0, 123), 4);
            numberMap.Add(Color.FromArgb(255, 129, 1, 2), 5);
            numberMap.Add(Color.FromArgb(255, 0, 128, 129), 6);
            numberMap.Add(Color.FromArgb(255, 0, 0, 0), 7);
            numberMap.Add(Color.FromArgb(255, 128, 128, 128), 8);
            //Form1_PreviewKeyDown(null, null);
        }

        Dictionary<Color, int> numberMap = new Dictionary<Color, int>();
        private void Form1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e == null || e.Control && e.KeyCode == Keys.T)
            {
                //Bitmap _bitmap = CaptureScreen();
                Bitmap _bitmap = new Bitmap(@"s5.png");
                Image<Bgr, Byte> original = new Image<Bgr, byte>(_bitmap);
                Image<Bgr, Byte> img = new Image<Bgr, byte>(_bitmap);

                // (1) Convert to gray, and threshold
                //Emgu.CV.CvInvoke.CvtColor(img, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                //Emgu.CV.CvInvoke.AdaptiveThreshold(img, img, 255, Emgu.CV.CvEnum.AdaptiveThresholdType.GaussianC, Emgu.CV.CvEnum.ThresholdType.BinaryInv, (int)numericUpDown1.Value, (int)numericUpDown2.Value);
                
                CvInvoke.CvtColor(img, img, Emgu.CV.CvEnum.ColorConversion.Bgr2Gray);
                CvInvoke.Canny(img, img, 90, 150, 3);

                //img.Save(@"C:\Users\PC\Desktop\canny1.png"); //save resulting image to desktop
                Mat kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
                CvInvoke.Dilate(img, img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

                kernel = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(5, 5), new Point(-1, -1));
                CvInvoke.Erode(img, img, kernel, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

                int n = 2;
                Mat vrtlin = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(1, n), new Point(-1, -1));
                Mat hrzlin = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(n, 1), new Point(-1, -1));

                CvInvoke.MorphologyEx(img, img, Emgu.CV.CvEnum.MorphOp.Close, vrtlin, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());
                CvInvoke.MorphologyEx(img, img, Emgu.CV.CvEnum.MorphOp.Close, hrzlin, new Point(-1, -1), 1, Emgu.CV.CvEnum.BorderType.Default, new MCvScalar());

                CvInvoke.Canny(img, img, 50, 150, 3);

                //img.Save(@"C:\Users\PC\Desktop\morph.png"); //save resulting image to desktop
                var lines = CvInvoke.HoughLinesP(img, 1, PI / 180, 80, 5, 20);//ilines=np.array([]), minLineLength=minLineLength,maxLineGap=80 (changed to 20, rbo to 80)

                int minx = int.MaxValue, miny = int.MaxValue, maxx = int.MinValue, maxy = int.MinValue;
                List<int> Tx = new List<int>();
                foreach (var line in lines)
                {
                    Tx.Add(line.P1.X);
                    if (line.P1.X > maxx) maxx = line.P1.X;
                    if (line.P2.X > maxx) maxx = line.P2.X;

                    if (line.P1.X < minx) minx = line.P1.X;
                    if (line.P2.X < minx) minx = line.P2.X;

                    if (line.P1.Y > maxy) maxy = line.P1.Y;
                    if (line.P2.Y > maxy) maxy = line.P2.Y;
                        
                    if (line.P1.Y < miny) miny = line.P1.Y;
                    if (line.P2.Y < miny) miny = line.P2.Y;

                    CvInvoke.Line(original, line.P1, line.P2, new MCvScalar(255, 0, 0), 2);
                }
                
                var _array = np.array(Tx.ToArray());
                int step = int.Parse(np.max(np.ediff1d(np.unique(np.sort(_array)).data)).ToString());

                int x = minx;
                int y = miny;
                int counter = 0;

                int[,] Matrix = new int[((maxx - minx )/ step), ((maxy - miny) / step)];
                int number = 0;
                int _y = 0;
                int _x = 0;
                while (x + step < maxx)
                {
                    _y = 0;
                    while (y + step < maxy)
                    {
                        Bitmap __bitmap = original.ToBitmap().Clone(new Rectangle(x, y, step, step), original.ToBitmap().PixelFormat);
                        number = 0; //BLANK
                        foreach (Color _color in numberMap.Keys)
                        {
                            if (imageColor(__bitmap, _color, 8))
                            {
                                number = numberMap[_color];
                                break;
                            }
                        }
                        if (number == 0 && imageColor(__bitmap, Color.FromArgb(255, 0, 0, 0), 10) && imageColor(__bitmap, Color.FromArgb(255, 254, 0, 0), 10))
                        {
                            number = -1; //FLAGGED
                        }
                        else if (number == 0 && imageColor(__bitmap, Color.FromArgb(255, 255, 255, 255), 30))
                        {
                            number = -2; //UNOPENED
                        }

                        Matrix[_x, _y] = number.ToString()[0];
                        counter++;
                        y += step;
                        _y++;
                    }
                    y = miny;
                    x += step;
                    _x++;
                }
               //original.Save(@"C:\Users\PC\Desktop\output.png"); //save resulting image to desktop
               //Process.Start(@"C:\Users\PC\Desktop\output.png");
                MessageBox.Show("OK");
            }
        }
        bool imageColor(Bitmap image, Color _color, int mincount = 8, int thres = 5)
        {
            Color temp;
            int count = 0;
            for (int i = 0; i < image.Height * image.Width; ++i)
            {
                int row = i / image.Height;
                int col = i % image.Width;
                if (row % 2 != 0) col = image.Width - col - 1;
                temp = image.GetPixel(col, row);
                if ((Math.Abs(temp.R - _color.R) < thres) && (Math.Abs(temp.G - _color.G) < thres) && (Math.Abs(temp.B - _color.B) < thres)) count++;
                if (count >= mincount) return true;
            }
            return false;

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Form1_PreviewKeyDown(sender, null);
        }
    }
}
