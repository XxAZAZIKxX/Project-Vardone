﻿using System;
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
            var dataContext = Program.DataContext;
            var tokens = dataContext.Tokens;
            tokens.Include(p => p.User).Load();
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
            var dataContext = Program.DataContext;
            var users = dataContext.Users;

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
            var dataContext = Program.DataContext;
            var friends = dataContext.FriendsList;
            friends.Include(p => p.FromUser).Load();
            friends.Include(p => p.ToUser).Load();
            try
            {
                var first = friends.First(p =>
                    p.FromUser.Id == idFirstUser && p.ToUser.Id == idSecondUser ||
                    p.FromUser.Id == idSecondUser && p.ToUser.Id == idFirstUser);
                return first.Confirmed;
            }
            catch
            {
                return false;
            }
        }

        public static bool CanGetUser(long idFirstUser, long idSecondUser)
        {
            if (!IsUserExists(idFirstUser) || !IsUserExists(idSecondUser)) return false;
            
            var res = false;
            
            var dataContext = Program.DataContext;
            var friendsList = dataContext.FriendsList;
            friendsList.Include(p => p.FromUser).Load();
            friendsList.Include(p => p.ToUser).Load();
            var users = dataContext.Users;

            var user1 = users.First(p => p.Id == idFirstUser);
            var user2 = users.First(p => p.Id == idSecondUser);

            try
            {
                var _ = friendsList.First(p => p.FromUser == user1 && p.ToUser == user2 || p.FromUser == user2 && p.ToUser == user1);
                res = true;
            }
            catch
            {
                // ignored
            }

            return res;
        }

        public static bool IsEmailAvailable(string email)
        {
            var dataContext = Program.DataContext;
            var users = dataContext.Users;
            try
            {
                var _ = users.First(p => p.Email == email);
                return false;
            }
            catch
            {
                return true;
            }
        }
    }
}