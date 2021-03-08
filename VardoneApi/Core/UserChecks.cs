using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Models.Users;

namespace VardoneApi.Core
{
    public abstract class UserChecks
    {
        public static bool CheckToken(TokenUserModel token)
        {
            if (token == null) return false;
            var tokens = Program.DataContext.Tokens;
            try
            {
                var _ = tokens.First(t => t.Token == token.Token && t.User.Username == token.Username);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsUserExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            var users = Program.DataContext.Users;

            try
            {
                var _ = users.First(p => p.Username == username);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsFriends(string firstUsername, string secondUsername)
        {
            var friends = Program.DataContext.FriendsList;
            friends.Include(p => p.From).Load();
            friends.Include(p => p.To).Load();
            try
            {
                var first = friends.First(p =>
                    p.From.Username == firstUsername && p.To.Username == secondUsername ||
                    p.From.Username == secondUsername && p.To.Username == firstUsername);
                return first.Confirmed;
            }
            catch
            {
                return false;
            }
        }
    }
}