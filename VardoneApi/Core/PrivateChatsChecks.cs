using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace VardoneApi.Core
{
    public abstract class PrivateChatsChecks
    {
        public static bool IsChatExists(string firstUsername, string secondUsername)
        {
            if (string.IsNullOrWhiteSpace(firstUsername)) return false;
            if (string.IsNullOrWhiteSpace(secondUsername)) return false;

            var chats = Program.DataContext.PrivateChats;
            chats.Include(p => p.From).Load();
            chats.Include(p => p.To).Load();
            try
            {
                var _ = chats.First(p =>
                   p.From.Username == firstUsername && p.To.Username == secondUsername ||
                   p.From.Username == secondUsername && p.To.Username == firstUsername);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsCanWriteMessage(string firstUsername, string secondUsername)
        {
            if (string.IsNullOrWhiteSpace(firstUsername)) return false;
            if (string.IsNullOrWhiteSpace(secondUsername)) return false;

            return UserChecks.IsFriends(firstUsername, secondUsername);
        }
    }
}