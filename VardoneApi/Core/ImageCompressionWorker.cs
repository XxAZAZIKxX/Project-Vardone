using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace VardoneApi.Core
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Interoperability", "CA1416:Проверка совместимости платформы", Justification = "<Ожидание>")]
    internal static class ImageCompressionWorker
    {
        public static byte[] VaryQualityLevel(byte[] image, long quality)
        {
            using (var stream = new MemoryStream(image))
            {
                using (var bmp1 = new Bitmap(stream))
                {
                    var jpgEncoder = GetEncoder(ImageFormat.Jpeg);

                    var myEncoderParameters = new EncoderParameters(1);

                    var myEncoderParameter = new EncoderParameter(Encoder.Quality, quality);
                    myEncoderParameters.Param[0] = myEncoderParameter;

                    using (var s = new MemoryStream())
                    {
                        bmp1.Save(s, jpgEncoder, myEncoderParameters);
                        return s.ToArray();
                    }
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format) => ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == format.Guid);
    }
}