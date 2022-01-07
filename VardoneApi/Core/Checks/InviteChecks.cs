using System;
using System.Linq;

namespace VardoneApi.Core.Checks
{
    public static class InviteChecks
    {
        public static bool IsInviteExists(string inviteCode)
        {
            var dataContext = Program.DataContext;
            var guildInvites = dataContext.GuildInvites;
            ClearExpiredInvites();
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

        public static void ClearExpiredInvites()
        {
            var dataContext = Program.DataContext;
            var guildInvites = dataContext.GuildInvites;
            foreach (var guildInvite in guildInvites)
            {
                if (DateTime.Now.Subtract(guildInvite.CreatedAt) >= TimeSpan.FromDays(1))
                    guildInvites.Remove(guildInvite);
            }
            dataContext.SaveChanges();
        }

        public static bool IsInviteExists(long inviteId)
        {
            var dataContext = Program.DataContext;
            var guildInvites = dataContext.GuildInvites;
            ClearExpiredInvites();
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