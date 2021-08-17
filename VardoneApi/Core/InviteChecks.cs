using System;
using System.Linq;

namespace VardoneApi.Core
{
    public abstract class InviteChecks
    {
        public static bool IsInviteExists(string inviteCode)
        {
            var dataContext = Program.DataContext;
            var guildInvites = dataContext.GuildInvites;
            try
            {
                var _ = guildInvites.First(p => p.InviteCode == inviteCode);
                if (DateTime.Now.Subtract(_.CreatedAt) <= TimeSpan.FromDays(1)) return true;

                guildInvites.Remove(_);
                dataContext.SaveChanges();
                return false;
            }
            catch 
            {
                return false;
            }
        }

        public static bool IsInviteExists(long inviteId)
        {
            var dataContext = Program.DataContext;
            var guildInvites = dataContext.GuildInvites;
            try
            {
                var _ = guildInvites.First(p => p.Id == inviteId);
                if (DateTime.Now.Subtract(_.CreatedAt) <= TimeSpan.FromDays(1)) return true;

                guildInvites.Remove(_);
                dataContext.SaveChanges();
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}