using System;
using VardoneApi.Core.Checks;
using VardoneApi.Entity.Models.Guilds;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;

namespace VardoneApi.Core.CreateHelpers
{
    internal static class UserCreateHelper
    {
        public static User GetUser(UsersTable user, bool onlyId = false)
        {
            if (user is null) return null;
            if (!UserChecks.IsUserExists(user.Id)) return null;
            var user1 = new User
            {
                UserId = user.Id,
                Username = user.Username
            };
            if (!onlyId)
            {
                user1.Description = user.Info?.Description;
                user1.Base64Avatar = user.Info?.Avatar is not null ? Convert.ToBase64String(user.Info.Avatar) : null;
            }
            return user1;
        }

        public static Member GetMember(GuildMembersTable member, bool onlyId = false)
        {
            if (member is null) return null;
            if (!UserChecks.IsUserExists(member.User.Id)) return null;
            return new Member
            {
                User = GetUser(member.User, onlyId),
                JoinDate = member.JoinDate,
                Guild = GuildCreateHelper.GetGuild(member.Guild, false,false , onlyId),
                NumberInvitedMembers = member.NumberOfInvitedMembers
            };
        }
    }
}