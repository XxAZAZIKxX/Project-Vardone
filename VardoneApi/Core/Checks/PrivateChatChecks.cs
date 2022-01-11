using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core.Checks
{
    internal static class PrivateChatChecks
    {
        public static bool IsChatExists(long idFirstUser, long idSecondUser)
        {
            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.FromUser).Load();
            chats.Include(p => p.ToUser).Load();
            return chats.Any(p => p.FromUser.Id == idFirstUser && p.ToUser.Id == idSecondUser || p.FromUser.Id == idSecondUser && p.ToUser.Id == idFirstUser);
        }
        public static bool IsChatExists(long chatId) => Program.DataContext.PrivateChats.Any(p => p.Id == chatId);

        public static bool IsCanWriteMessage(long idFirstUser, long idSecondUser) => UserChecks.IsFriends(idFirstUser, idSecondUser) || UserChecks.DoUsersHaveSharedGuilds(idFirstUser, idSecondUser);

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