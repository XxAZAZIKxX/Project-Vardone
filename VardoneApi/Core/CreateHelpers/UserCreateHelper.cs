using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.Checks;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;

namespace VardoneApi.Core.CreateHelpers
{
    internal static class UserCreateHelper
    {
        public static User GetUser(long userId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var users = dataContext.Users;
            users.Include(p => p.Info).Load();
            var user = users.FirstOrDefault(p => p.Id == userId);
            if (user is null) return null;
            if (!UserChecks.IsUserExists(user.Id)) return null;
            var getUser = new User
            {
                UserId = user.Id,
                Username = user.Username
            };
            if (!onlyId)
            {
                getUser.Description = user.Info?.Description;
                getUser.Base64Avatar = user.Info?.Avatar is not null ? Convert.ToBase64String(user.Info.Avatar) : null;
            }
            return getUser;
        }

        public static Member GetMember(long memberId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var guildMembers = dataContext.GuildMembers;
            guildMembers.Include(p => p.User).Load();
            guildMembers.Include(p => p.Guild).Load();
            var member = guildMembers.FirstOrDefault(p => p.Id == memberId);
            if (member is null) return null;
            if (!UserChecks.IsUserExists(member.User.Id)) return null;
            return new Member
            {
                User = GetUser(member.User.Id, onlyId),
                JoinDate = member.JoinDate,
                NumberInvitedMembers = member.NumberOfInvitedMembers,
                Guild = GuildCreateHelper.GetGuild(member.Guild.Id, false, false, true)
            };
        }
        public static Member GetMember(long userId, long guildId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var members = dataContext.GuildMembers;
            members.Include(p => p.Guild).Load();
            members.Include(p => p.User).Load();
            var member = members.FirstOrDefault(p => p.Guild.Id == guildId && p.User.Id == userId);
            return member is null ? null : GetMember(member.Id, onlyId);
        }

        public static BannedMember GetBannedMember(long bannedMemberId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var bannedGuildMembers = dataContext.BannedGuildMembers;
            bannedGuildMembers.Include(p => p.BannedByUser).Load();
            bannedGuildMembers.Include(p => p.BannedUser).Load();
            bannedGuildMembers.Include(p => p.Guild).Load();
            var bannedMember = bannedGuildMembers.FirstOrDefault(p => p.Id == bannedMemberId);
            if (bannedMember is null) return null;
            return new BannedMember
            {
                Guild = GuildCreateHelper.GetGuild(bannedMember.Guild.Id, false, false, true),
                BanDateTime = bannedMember.BanDate,
                BannedByUser = GetUser(bannedMember.BannedByUser.Id, onlyId),
                BannedUser = GetUser(bannedMember.BannedUser.Id, onlyId),
                Reason = bannedMember.Reason
            };
        }
        public static BannedMember GetBannedMember(long userId, long guildId, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var bannedGuildMembers = dataContext.BannedGuildMembers;
            bannedGuildMembers.Include(p => p.BannedUser);
            bannedGuildMembers.Include(p => p.Guild);

            var bannedMember = bannedGuildMembers.FirstOrDefault(p => p.BannedUser.Id == userId && p.Guild.Id == guildId);
            return bannedMember is null ? null : GetBannedMember(bannedMember.Id, onlyId);
        }
    }
}