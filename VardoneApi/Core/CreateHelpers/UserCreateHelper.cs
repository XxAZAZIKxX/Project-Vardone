using System;
using VardoneApi.Entity.Models.Guilds;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Guild;

namespace VardoneApi.Core.CreateHelpers
{
    public abstract class UserCreateHelper
    {
        public static User GetUser(UsersTable user)
        {
            return new User
            {
                UserId = user.Id,
                Username = user.Username,
                Description = user.Info?.Description,
                Base64Avatar = user.Info?.Avatar is not null ? Convert.ToBase64String(user.Info.Avatar) : null,
            };
        }

        public static Member GetMember(GuildMembersTable member)
        {
            return new Member
            {
                User = GetUser(member.User),
                JoinDate = member.JoinDate,
                Guild = GuildCreateHelper.GetGuild(member.Guild),
                NumberInvitedMembers = member.NumberOfInvitedMembers
            };
        }
    }
}