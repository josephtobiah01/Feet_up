using System.Drawing.Imaging;
using System.Drawing;
using static System.Runtime.InteropServices.JavaScript.JSType;
using FitappAdminWeb.Net7.Classes.Constants;

namespace FitappAdminWeb.Net7.Classes.Utilities
{
    public static class ImageUtil
    {
        public static Image ImageResize(Image srcImage, int newWidth, int newHeight)
        {
            Bitmap bitmap = new Bitmap(newWidth, newHeight);

            if (bitmap.PixelFormat == PixelFormat.Format1bppIndexed ||
                bitmap.PixelFormat == PixelFormat.Format4bppIndexed ||
                bitmap.PixelFormat == PixelFormat.Undefined ||
                bitmap.PixelFormat == PixelFormat.DontCare ||
                bitmap.PixelFormat == PixelFormat.Format16bppArgb1555 ||
                bitmap.PixelFormat == PixelFormat.Format16bppGrayScale)
            {
                throw new NotSupportedException("Pixel Format not supported");
            }

            using (Graphics gImage = Graphics.FromImage(bitmap))
            {
                gImage.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                gImage.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                gImage.DrawImage(srcImage, 0, 0, bitmap.Width, bitmap.Height);
            }
            return bitmap;
        }

        public static byte[] GetBytesFromImage(Image i)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(i, typeof(byte[]));
            return xByte;
        }

        public static byte[]? GetBytesFromImageFormFile(IFormFile formFile, int maxSize, bool resizeImage = false, int resizeWidth = 0, int resizeHeight = 0)
        {
            byte[]? imageBytes = null;

            if (formFile != null &&
                formFile.Length > 0 &&
                formFile.Length < maxSize &&
                formFile.ContentType.StartsWith("image/"))
            {
                using (var fileStream = formFile.OpenReadStream())
                {
                    Image img = Image.FromStream(fileStream);
                    if (img.Width == resizeWidth && img.Height == resizeHeight)
                    {
                        imageBytes = GetBytesFromImage(img);
                    }
                    else
                    {
                        //resize the image if it does not meet the noted dimensions
                        Image resizeImg = ImageResize(img, resizeWidth, resizeHeight);
                        imageBytes = GetBytesFromImage(resizeImg);
                    }
                }
            }

            return imageBytes;
        }
    }
}
