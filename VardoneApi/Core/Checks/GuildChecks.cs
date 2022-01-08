using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core.Checks
{
    public static class GuildChecks
    {
        public static bool IsGuildExists(long guildId) => Program.DataContext.Guilds.Any(p => p.Id == guildId);

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
            return guildMembers.Any(p => p.User.Id == userId && p.Guild.Id == guildId);
        }
    }
}