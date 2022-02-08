using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.CreateHelpers;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.TcpModels;

namespace VardoneApi.Core.Checks
{
    internal static class InviteChecks
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
            guildInvites.Include(p => p.Guild).Load();
            guildInvites.Include(p => p.Guild.Owner).Load();
            foreach (var guildInvite in guildInvites)
            {
                if (DateTime.Now.Subtract(guildInvite.CreatedAt) < TimeSpan.FromDays(1)) continue;
                guildInvites.Remove(guildInvite);
                Program.TcpServer.SendMessageTo(guildInvite.Guild.Owner.Id, new TcpResponseModel
                {
                    type = TypeTcpResponse.DeleteGuildInvite,
                    data = new GuildInvite
                    {
                        InviteId = guildInvite.Id,
                        Guild = GuildCreateHelper.GetGuild(guildInvite.Guild.Id, false, false, true)
                    }
                });
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