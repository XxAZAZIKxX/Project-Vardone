using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace VardoneApi.Core
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
    internal static class ImageWorker
    {
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