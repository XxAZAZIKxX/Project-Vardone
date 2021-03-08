using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core
{
    public abstract class PrivateChatsChecks
    {
        public static bool IsChatExists(long idFirstUser, long idSecondUser)
        {

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.From).Load();
            chats.Include(p => p.To).Load();
            try
            {
                var _ = chats.First(p =>
                   p.From.Id == idFirstUser && p.To.Id == idSecondUser ||
                   p.From.Id == idSecondUser && p.To.Id == idFirstUser);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsCanWriteMessage(long idFirstUser, long idSecondUser) => UserChecks.IsFriends(idFirstUser, idSecondUser);
    }
}