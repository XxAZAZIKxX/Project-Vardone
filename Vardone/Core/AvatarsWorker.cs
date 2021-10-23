using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Vardone.Pages;

namespace Vardone.Core
{
    public abstract class AvatarsWorker
    {
        public static Dictionary<long, BitmapImage> UserAvatars { get; } = new();

        /// <summary>
        /// Аватар по умолчанию
        /// </summary>
        public static BitmapImage DefaultAvatar { get; } = ImageWorker.BytesToBitmapImage(File.ReadAllBytes(MainWindow.Path + @"\resources\contentRes\avatar.jpg"));

        private static readonly object Locker = new();

        /// <summary>
        /// Получить аватар пользователя через id
        /// </summary>
        /// <param name="userId" />
        /// <returns>Аватар пользователя</returns>
        public static BitmapImage GetAvatarUser(long userId)
        {
            if (MainPage.Client is null) return null;
            UpdateAvatarUser(userId);
            return UserAvatars[userId];
        }

        /// <summary>
        /// Обновить сохраненный аватар
        /// </summary>
        /// <param name="userId" />
        public static void UpdateAvatarUser(long userId)
        {
            if (MainPage.Client is null) return;
            var base64 = MainPage.Client.GetUser(userId)?.Base64Avatar;
            lock (Locker)
            {
                UserAvatars[userId] = base64 is null ? DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(base64));
            }
        }
    }
}