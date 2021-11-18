using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore.Internal;
using Vardone.Pages;

namespace Vardone.Core
{
    public abstract class AvatarsWorker
    {
        private static Dictionary<long, BitmapImage> UserAvatars { get; } = new();
        private static Dictionary<long, BitmapImage> GuildAvatars { get; } = new();
        private static BitmapImage DefaultAvatar { get; } = ImageWorker.BytesToBitmapImage(File.ReadAllBytes(MainWindow.Path + @"\resources\contentRes\avatar.jpg"));
        private static readonly object Locker = new();

        public static BitmapImage GetAvatarUser(long userId)
        {
            if (MainPage.Client is null) return null;
            UpdateAvatarUser(userId);
            return UserAvatars[userId];
        }
        public static void UpdateAvatarUser(long userId)
        {
            if (MainPage.Client is null) return;
            var base64 = MainPage.Client.GetUser(userId)?.Base64Avatar;
            lock (Locker)
            {
                UserAvatars[userId] = base64 is null ? DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(base64));
            }
        }
        public static BitmapImage GetGuildAvatar(long guildId)
        {
            if (MainPage.Client is null) return null;
            UpdateGuildAvatar(guildId);
            return GuildAvatars[guildId];
        }
        public static void UpdateGuildAvatar(long guildId)
        {
            if (MainPage.Client is null) return;
            var base64 = MainPage.Client.GetGuilds().FirstOr(p => p.GuildId == guildId, null!)?.Base64Avatar;
            lock (Locker)
            {
                GuildAvatars[guildId] = base64 is null ? DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(base64));
            }
        }

        public static void ClearAll()
        {
            UserAvatars.Clear();
            GuildAvatars.Clear();
            GC.Collect();
        }
    }
}