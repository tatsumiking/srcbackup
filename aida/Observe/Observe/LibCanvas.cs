using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Observe
{
    public class LibCanvas
    {
        private double m_dFontSize;
        private string m_sFontName;
        private Brush m_brushFill;
        private Brush m_brushStroke;
        private double m_dStrokeThick;

        public LibCanvas()
        {
            m_brushStroke = Brushes.Black;
            m_dStrokeThick = 1.0d;
            m_sFontName = "ＭＳ ゴシック";
            m_dFontSize = 16.0;
        }
        public void setFontSize(double size)
        {
            m_dFontSize = size;
        }
        public void setFillBrush(Brush brush)
        {
            m_brushFill = brush;
        }
        public void setStrokeBrush(Brush brush, double thick)
        {
            m_brushStroke = brush;
            m_dStrokeThick = thick;
        }
        public Brush createBruch(int rgb)
        {
            int r, g, b;
            Color clr;
            Brush brush;

            if (rgb == -1)
            {
                return (null);
            }
            else
            {
                r = (rgb >> 16) & 0x00ff;
                g = (rgb >> 8) & 0x00ff;
                b = (rgb) & 0x00ff;
                clr = Color.FromArgb(255, (byte)r, (byte)g, (byte)b);
                brush = new SolidColorBrush(clr);
                return (brush);
            }

        }
        public void setImageSource(Canvas canvas, Image img, string sFileName)
        {
            string err;
            FileStream fs;
            long size;
            byte[] data;
            MemoryStream stream;
            BitmapImage bmpImage;
            try
            {
                fs = new FileStream(sFileName, FileMode.Open, FileAccess.Read);
                size = fs.Seek(0L, SeekOrigin.End);
                fs.Seek(0L, SeekOrigin.Begin);
                data = new byte[size];
                fs.Read(data, 0, (int)size);
                fs.Close();
                fs.Dispose();

                stream = new MemoryStream();
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);

                bmpImage = new BitmapImage();
                bmpImage.BeginInit();
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.StreamSource = stream;
                bmpImage.EndInit();
                bmpImage.Freeze();

                img.Source = bmpImage;

                stream.Close();
                stream.Dispose();
                canvas.Children.Add(img);
            }
            catch (Exception exp)
            {//例外発生
                err = exp.Message;
                MessageBox.Show(err.ToString());
            }
        }
        public Image drawImage(Canvas canvas, double x, double y, double w, double h, string sFileName)
        {
            Image img;

            img = CreateImage(x, y, w, h, sFileName);
            if (img != null)
            {
                canvas.Children.Add(img);
            }
            return (img);
        }
        public Canvas drawCenterText(Canvas canvas, double sx, double sy, double wd, double hi, double adx, double ady, string str)
        {
            TextBlock tb;
            Canvas cvText = new Canvas();
            Canvas.SetLeft(cvText, sx);
            Canvas.SetTop(cvText, sy);
            cvText.Width = wd;
            cvText.Height = hi;

            tb = new TextBlock();
            Canvas.SetLeft(tb, adx);
            Canvas.SetTop(tb, ady);
            tb.Width = wd;
            tb.TextAlignment = TextAlignment.Center;
            tb.Text = str;
            tb.FontFamily = new FontFamily(m_sFontName);
            tb.FontSize = m_dFontSize;
            tb.Foreground = m_brushFill;

            cvText.Children.Add(tb);
            canvas.Children.Add(cvText);
            return (cvText);
        }
        public Canvas drawLeftText(Canvas canvas, double sx, double sy, double wd, double hi, double adx, double ady, string str)
        {
            TextBlock tb;
            Canvas cvText = new Canvas();
            Canvas.SetLeft(cvText, sx);
            Canvas.SetTop(cvText, sy);
            cvText.Width = wd;
            cvText.Height = hi;

            tb = new TextBlock();
            Canvas.SetLeft(tb, 0);
            Canvas.SetTop(tb, 7);
            tb.Width = wd;
            tb.TextAlignment = TextAlignment.Left;
            tb.Text = str;
            tb.FontFamily = new FontFamily(m_sFontName);
            tb.FontSize = m_dFontSize;
            tb.Foreground = m_brushFill;

            cvText.Children.Add(tb);
            canvas.Children.Add(cvText);
            return (cvText);
        }
        public Canvas drawText(Canvas canvas, double sx, double sy, double wd, double hi, string str)
        {
            TextBlock tb;
            Canvas cvText = new Canvas();
            Canvas.SetLeft(cvText, sx);
            Canvas.SetTop(cvText, sy);
            cvText.Width = wd;
            cvText.Height = hi;
            tb = CreateTextBlock(0, 0, str);
            tb.Foreground = m_brushFill;
            cvText.Children.Add(tb);
            canvas.Children.Add(cvText);
            return (cvText);
        }
        // Imageオブジェクトの生成
        private Image CreateImage(double x, double y, double width, double height, string sFileName)
        {
            Image image;
            string err;

            image = new Image();
            Canvas.SetLeft(image, x);
            Canvas.SetTop(image, y);
            image.Width = width;
            image.Stretch = Stretch.Fill;

            BitmapImage bmpImage;
            FileStream stream;
            try
            {
                bmpImage = new BitmapImage();
                stream = File.OpenRead(sFileName);
                bmpImage.BeginInit();
                bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                bmpImage.CreateOptions = BitmapCreateOptions.None;
                bmpImage.StreamSource = stream;
                bmpImage.EndInit();
                bmpImage.Freeze();
                stream.Close();
                stream.Dispose();
                image.Source = bmpImage;
            }
            catch (Exception exp)
            {//例外発生
                err = exp.Message;
                // MessageBox.Show(err.ToString());
                image = null;
            }
            return image;
        }
        public TextBlock CreateTextBlock(double x, double y, string text)
        {
            TextBlock tb = new TextBlock();
            Canvas.SetLeft(tb, x);
            Canvas.SetTop(tb, y);
            tb.Text = text;
            tb.FontFamily = new FontFamily(m_sFontName);
            tb.FontSize = m_dFontSize;
            return tb;
        }
        public void drawLine(Canvas canvas, double sx, double sy, double ex, double ey)
        {
            Line line;

            line = CreateLine(sx, sy, ex, ey);
            canvas.Children.Add(line);
        }
        public Rectangle drawBoxs(Canvas canvas, double sx, double sy, double wd, double hi)
        {
            Rectangle rect;

            rect = CreateRect(sx, sy, wd, hi);
            canvas.Children.Add(rect);
            return (rect);
        }
        public void drawEllipse(Canvas canvas, double sx, double sy, double wd, double hi)
        {
            Ellipse elli;

            elli = CreateEllipse(sx, sy, wd, hi);
            canvas.Children.Add(elli);
        }
        private Line CreateLine(double x1, double y1, double x2, double y2)
        {
            Line line = new Line();
            line.X1 = x1;
            line.Y1 = y1;
            line.X2 = x2;
            line.Y2 = y2;
            line.Stroke = m_brushStroke;
            line.StrokeThickness = m_dStrokeThick;
            return line;
        }

        // Rectangleオブジェクトの生成
        private Rectangle CreateRect(double x, double y, double width, double height)
        {
            Rectangle rect = new Rectangle();
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
            rect.Width = width;
            rect.Height = height;
            rect.Stroke = m_brushStroke;
            rect.StrokeThickness = m_dStrokeThick;
            rect.Fill = m_brushFill;
            return rect;
        }

        // Ellipseオブジェクトの生成
        private Ellipse CreateEllipse(double x, double y, double width, double height)
        {
            Ellipse ellipse = new Ellipse();
            Canvas.SetLeft(ellipse, x);
            Canvas.SetTop(ellipse, y);
            ellipse.Width = width;
            ellipse.Height = height;
            ellipse.Stroke = m_brushStroke;
            ellipse.StrokeThickness = m_dStrokeThick;
            ellipse.Fill = m_brushFill;
            return ellipse;

        }
    }
}
