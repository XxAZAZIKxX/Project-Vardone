﻿using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace Vardone.Core
{
    public abstract class ImageWorker
    {
        public static BitmapImage BytesToBitmapImage(byte[] array)
        {
            if (array is null) return null;
            using var ms = new MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }

        public static byte[] BitmapImageToBytes(BitmapImage image)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));
            using var ms = new MemoryStream();
            encoder.Save(ms);
            return ms.ToArray();
        }
    }
}