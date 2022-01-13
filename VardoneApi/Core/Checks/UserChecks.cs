using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Core.Checks
{
    internal static class UserChecks
    {
        public static bool CheckToken(UserTokenModel token)
        {
            if (token == null) return false;
            var tokens = Program.DataContext.Tokens;
            tokens.Include(p => p.User).Load();
            return tokens.Any(t => t.Token == token.Token && t.User.Id == token.UserId);
        }

        public static bool IsUserExists(string username) => username is not null && Program.DataContext.Users.Any(p => p.Username == username);

        public static bool IsUserExists(long id) => Program.DataContext.Users.Any(p => p.Id == id);

        public static bool IsFriends(long idFirstUser, long idSecondUser)
        {
            var friends = Program.DataContext.FriendsList;
            friends.Include(p => p.FromUser).Load();
            friends.Include(p => p.ToUser).Load();
            return friends.Any(p => p.FromUser.Id == idFirstUser && p.ToUser.Id == idSecondUser || p.FromUser.Id == idSecondUser && p.ToUser.Id == idFirstUser);
        }

        public static bool IsFriends(long idFirstUser, string usernameSecondUser)
        {
            if (usernameSecondUser == null) return false;
            var friends = Program.DataContext.FriendsList;
            friends.Include(p => p.FromUser).Load();
            friends.Include(p => p.ToUser).Load();
            return friends.Any(p => (p.FromUser.Id == idFirstUser && p.ToUser.Username == usernameSecondUser || p.FromUser.Username == usernameSecondUser && p.ToUser.Id == idFirstUser) && p.Confirmed);
        }

        public static bool IsFriendRequestExists(long idFirstUser, long idSecondUser)
        {
            if (!IsUserExists(idFirstUser) || !IsUserExists(idSecondUser)) return false;
            var dataContext = Program.DataContext;
            var friendsList = dataContext.FriendsList;
            friendsList.Include(p => p.FromUser).Load();
            friendsList.Include(p => p.ToUser).Load();
            var users = dataContext.Users;

            var user1 = users.First(p => p.Id == idFirstUser);
            var user2 = users.First(p => p.Id == idSecondUser);
            return friendsList.Any(p => p.FromUser == user1 && p.ToUser == user2 || p.FromUser == user2 && p.ToUser == user1);
        }

        public static bool DoUsersHaveSharedGuilds(long idFirstUser, long idSecondUser)
        {
            if (!IsUserExists(idFirstUser) || !IsUserExists(idSecondUser)) return false;
            var dataContext = Program.DataContext;

            var users = dataContext.Users;

            var guildsMembers = dataContext.GuildMembers;
            guildsMembers.Include(p => p.Guild).Load();
            guildsMembers.Include(p => p.User).Load();

            var user1 = users.First(p => p.Id == idFirstUser);
            var user2 = users.First(p => p.Id == idSecondUser);

            var count = guildsMembers.Where(p => p.User == user1 || p.User == user2).ToList().GroupBy(p => p.Guild).Count();

            return count > 0;
        }

        public static bool CanGetUser(long idFirstUser, long idSecondUser) => idFirstUser == idSecondUser || IsFriendRequestExists(idFirstUser, idSecondUser) || DoUsersHaveSharedGuilds(idFirstUser, idSecondUser);

        public static bool IsEmailAvailable(string email)
        {
            if (email is null) return false;
            return !Program.DataContext.Users.Any(p => p.Email == email);
        }
    }
}