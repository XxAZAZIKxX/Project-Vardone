using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core
{
    public abstract class PrivateChatsChecks
    {
        public static bool IsChatExists(long idFirstUser, long idSecondUser)
        {

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.FromUser).Load();
            chats.Include(p => p.ToUser).Load();
            try
            {
                var _ = chats.First(p =>
                   p.FromUser.UserId == idFirstUser && p.ToUser.UserId == idSecondUser ||
                   p.FromUser.UserId == idSecondUser && p.ToUser.UserId == idFirstUser);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsChatExists(long chatId)
        {

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.FromUser).Load();
            chats.Include(p => p.ToUser).Load();
            try
            {
                var _ = chats.First(p =>p.ChatId == chatId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsCanReadMessages(long userId, long chatId)
        {
            if (!IsChatExists(chatId)) return false;
            var chats = Program.DataContext.PrivateChats;
            chats.Include(p=>p.FromUser).Load();
            chats.Include(p=>p.ToUser).Load();

            var chat = chats.First(p => p.ChatId == chatId);
            return chat.FromUser.UserId == userId || chat.ToUser.UserId == userId;
        }
        public static bool IsCanWriteMessage(long idFirstUser, long idSecondUser) => UserChecks.IsFriends(idFirstUser, idSecondUser);
    }
}