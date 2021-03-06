using System;
using System.Linq;
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
                var unused = tokens.First(t => t.Token == token.Token && t.User.Username == token.Username);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool UserExists(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;

            var users = Program.DataContext.Users;

            try
            {
                var unused = users.First(p => p.Username == username);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}