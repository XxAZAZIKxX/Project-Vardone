using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core.Checks
{
    public abstract class PrivateChatChecks
    {
        public static bool IsChatExists(long idFirstUser, long idSecondUser)
        {
            var dataContext = Program.DataContext;
            var chats = dataContext.PrivateChats;
            chats.Include(p => p.FromUser).Load();
            chats.Include(p => p.ToUser).Load();
            try
            {
                var _ = chats.First(p =>
                   p.FromUser.Id == idFirstUser && p.ToUser.Id == idSecondUser ||
                   p.FromUser.Id == idSecondUser && p.ToUser.Id == idFirstUser);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsChatExists(long chatId)
        {
            var dataContext = Program.DataContext;
            var chats = dataContext.PrivateChats;
            try
            {
                var _ = chats.First(p => p.Id == chatId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsCanWriteMessage(long idFirstUser, long idSecondUser) =>
            UserChecks.IsFriends(idFirstUser, idSecondUser) ||
            UserChecks.DoUsersHaveSharedGuilds(idFirstUser, idSecondUser);

        public static bool IsCanManageChat(long userId, long chatId)
        {
            if (!IsChatExists(chatId)) return false;
            if (!UserChecks.IsUserExists(userId)) return false;

            var dataContext = Program.DataContext;
            var privateChats = dataContext.PrivateChats;
            privateChats.Include(p => p.FromUser).Load();
            privateChats.Include(p => p.ToUser).Load();
            var chat = privateChats.First(p => p.Id == chatId);
            return chat.FromUser.Id == userId || chat.ToUser.Id == userId;
        }
    }
}