using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows;

namespace AutomationHelper
{
    public class ImageHelper
    {
        /// <summary>
        /// take screenshot and save to a file
        /// </summary>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public static string TakeScreenshot(string filepath)
        {
            if (string.IsNullOrEmpty(filepath))
                filepath = string.Format(@"{0}\{1}.png", Environment.CurrentDirectory, TextHelper.RandomString(10));
            
            var bmp = TakeSnapshot();
            bmp.Save(filepath, ImageFormat.Png);

            return filepath;
        }

        /// <summary>
        /// take snapshot for primary screen
        /// </summary>
        /// <returns></returns>
        public static Bitmap TakeSnapshot()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                                    Screen.PrimaryScreen.Bounds.Height,
                                    PixelFormat.Format32bppPArgb);

            // Create a graphic for drawing from bmp
            var screenG = Graphics.FromImage(bmp);
            screenG.CopyFromScreen(Screen.PrimaryScreen.Bounds.X,
                                   Screen.PrimaryScreen.Bounds.Y,
                                   0, 0,
                                   Screen.PrimaryScreen.Bounds.Size,
                                   CopyPixelOperation.SourceCopy);

            return bmp;
        }

        /// <summary>
        /// take snapshot for specified size and position
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="recSize"></param>
        /// <returns></returns>
        public static Bitmap TakeSnapshot(int width, int height, int x, int y, System.Drawing.Size recSize)
        {
            Bitmap bmp = null;
            
            try
            {
                // Create a bitmap for save
                bmp = new Bitmap(width, height, PixelFormat.Format32bppPArgb);

                // Create a graphic for drawing from bmp
                var screenG = Graphics.FromImage(bmp);
                screenG.CopyFromScreen(x, y, 0, 0, recSize, CopyPixelOperation.SourceCopy);
            }
            catch
            {                
            }

            return bmp;
        }

        /// <summary>
        /// transfer image to base64 string
        /// </summary>
        /// <param name="image"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string ImageToBase64(Image image, ImageFormat format)
        {
            using (var ms = new MemoryStream())
            {
            // Convert Image to byte[]
            image.Save(ms, format);
            var imageBytes = ms.ToArray();

            // Convert byte[] to Base64 String
            var base64String = Convert.ToBase64String(imageBytes);
            return base64String;
            }
        }

        /// <summary>
        /// transfer base64 string to image
        /// </summary>
        /// <param name="base64String"></param>
        /// <returns></returns>
        public static Image Base64ToImage(string base64String)
        {
            // Convert Base64 String to byte[]
            var imageBytes = Convert.FromBase64String(base64String);
            var ms = new MemoryStream(imageBytes, 0, imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            if (ms.Length > 0)
                return Image.FromStream(ms, true);
            return null;
        }

        /// <summary>
        /// convert bitmap to bitmapsource
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static BitmapSource ToBitmapSource(Bitmap source)
        {
            BitmapSource bitSrc;

            if (source == null)
                return null;

            var hBitmap = source.GetHbitmap();

            try
            {
                bitSrc = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            catch (Win32Exception)
            {
                bitSrc = null;
            }

            return bitSrc;
        }

        /// <summary>
        /// resize bitmap with specified size
        /// </summary>
        /// <param name="sourceBMP"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap ResizeBitmap(Bitmap sourceBMP, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
                g.DrawImage(sourceBMP, 0, 0, width, height);
            return result;
        }
    }
}
