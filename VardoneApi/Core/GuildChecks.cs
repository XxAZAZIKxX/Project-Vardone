using System;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core
{
    public abstract class GuildChecks
    {
        public static bool IsGuildExists(long guildId)
        {
            try
            {
                var dataContext = Program.DataContext;
                var guilds = dataContext.Guilds;
                var _ = guilds.First(p => p.Id == guildId);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsUserOwner(long userId, long guildId)
        {
            if (!UserChecks.IsUserExists(userId)) return false;
            if (!IsGuildExists(guildId)) return false;

            var dataContext = Program.DataContext;
            var guilds = dataContext.Guilds;
            guilds.Include(p => p.Owner).Load();
            var first = guilds.First(p => p.Id == guildId);
            return first.Owner.Id == userId;
        }

        public static bool IsUserMember(long userId, long guildId)
        {
            if (!UserChecks.IsUserExists(userId)) return false;
            if (!IsGuildExists(guildId)) return false;
            var dataContext = Program.DataContext;
            var guildMembers = dataContext.GuildMembers;
            guildMembers.Include(p => p.Guild).Load();
            guildMembers.Include(p => p.User).Load();
            try
            {
                var _ = guildMembers.First(p => p.User.Id == userId && p.Guild.Id == guildId);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}