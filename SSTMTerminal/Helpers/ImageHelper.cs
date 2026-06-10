using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SSTMTerminal.Helpers
{
    public static class ImageHelper
    {
        public static ImageSource BitmapToImageSource(this Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            IntPtr hBitmap = bitmap.GetHbitmap();
            ImageSource imageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            if (!SystemHelper.DeleteObject(hBitmap))
            {
                throw new System.ComponentModel.Win32Exception();
            }

            return imageSource;
        }

        public static ImageSource GetImageSource(string uri)
        {
            ImageSource wpfBitmap = null;

            try
            {
                using (Bitmap bitmap = (Bitmap)Image.FromFile(uri))
                {
                    IntPtr hBitmap = bitmap.GetHbitmap();
                    wpfBitmap = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       hBitmap,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());

                    if (!SystemHelper.DeleteObject(hBitmap))
                    {
                        throw new System.ComponentModel.Win32Exception();
                    }
                }
            }
            catch (Exception)
            {
                wpfBitmap = null;
            }

            return wpfBitmap;
        }
    }
}