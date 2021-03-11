using System.Windows.Media.Imaging;

namespace Vardone.Core
{
    public abstract class ImageWorker
    {
        public static BitmapImage ByteArrayToImage(byte[] array)
        {
            if (array is null) return null;
            using var ms = new System.IO.MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}