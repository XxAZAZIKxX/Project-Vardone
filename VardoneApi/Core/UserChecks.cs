using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneApi.Core
{
    public abstract class UserChecks
    {
        public static bool CheckToken(UserTokenModel token)
        {
            if (token == null) return false;
            var tokens = Program.DataContext.Tokens;
            tokens.Include(p=>p.User).Load();
            try
            {
                var _ = tokens.First(t => t.Token == token.Token && t.User.Id == token.UserId);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsUserExists(long id)
        {

            var users = Program.DataContext.Users;

            try
            {
                var _ = users.First(p => p.Id == id);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsFriends(long idFirstUser, long idSecondUser)
        {
            var friends = Program.DataContext.FriendsList;
            friends.Include(p => p.From).Load();
            friends.Include(p => p.To).Load();
            try
            {
                var first = friends.First(p =>
                    p.From.Id == idFirstUser && p.To.Id == idSecondUser ||
                    p.From.Id == idSecondUser && p.To.Id == idFirstUser);
                return first.Confirmed;
            }
            catch
            {
                return false;
            }
        }
    }
}