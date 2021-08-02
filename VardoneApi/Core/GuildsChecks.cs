using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core
{
    public abstract class GuildsChecks
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
    }
}