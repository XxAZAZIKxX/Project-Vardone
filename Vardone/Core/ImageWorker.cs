using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace Vardone.Core
{
    public static class ImageWorker
    {
        /// <summary>
        /// Перевести массив байтов в BitmapImage
        /// </summary>
        /// <param name="array" />
        /// <returns>BitmapImage</returns>
        public static BitmapImage BytesToBitmapImage(byte[] array)
        {
            if (array is null) return null;
            if (!IsImage(array)) return null;
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        /// <summary>
        /// Перевести BitmapImage в массив байтов 
        /// </summary>
        /// <param name="image" />
        /// <returns>Массив байтов</returns>
        public static byte[] BitmapImageToBytes(BitmapImage image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }

        public static bool IsImage(byte[] bytes)
        {
            if (bytes is null) return false;
            using var stream = new MemoryStream(bytes);
            try
            {
                Image image = new Bitmap(stream);
                if (!image.RawFormat.Equals(ImageFormat.Jpeg) && !image.RawFormat.Equals(ImageFormat.Png))
                    throw new Exception();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static byte[] CompressImageQualityLevel(byte[] image, long quality)
        {
            using var stream = new MemoryStream(image);
            using var bmp1 = new Bitmap(stream);
            var encoder = GetEncoder(ImageFormat.Png.Equals(bmp1.RawFormat) ? ImageFormat.Png : ImageFormat.Jpeg);

            var myEncoderParameters = new EncoderParameters(1);

            var myEncoderParameter = new EncoderParameter(Encoder.Quality, quality);
            myEncoderParameters.Param[0] = myEncoderParameter;

            using var s = new MemoryStream();
            bmp1.Save(s, encoder, myEncoderParameters);
            return s.ToArray();
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format) => ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
    }
}