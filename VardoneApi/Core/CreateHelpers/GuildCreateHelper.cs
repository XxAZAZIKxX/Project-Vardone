using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.Checks;
using VardoneEntities.Entities.Guild;

namespace VardoneApi.Core.CreateHelpers
{
    internal static class GuildCreateHelper
    {
        public static Channel[] GetGuildChannels(long guildId, bool onlyId = false)
        {
            if (!GuildChecks.IsGuildExists(guildId)) return null;

            var channels = Program.DataContext.Channels;
            channels.Include(p => p.Guild).Load();
            channels.Include(p => p.Guild.Info).Load();

            var channelsList = new List<Channel>();

            foreach (var channel in channels.Where(p => p.Guild.Id == guildId))
            {
                channelsList.Add(GetChannel(channel.Id));
            }
            return channelsList.ToArray();
        }

        public static Guild GetGuild(long guildId, bool withChannels = true, bool withOwner = true, bool onlyId = false)
        {
            var dataContext = Program.DataContext;
            var guilds = dataContext.Guilds;
            guilds.Include(p=>p.Owner).Load();
            guilds.Include(p=>p.Info).Load();
            var members = dataContext.GuildMembers;
            members.Include(p => p.User).Load();
            var guild = guilds.FirstOrDefault(p => p.Id == guildId);
            if (guild is null) return null;

            var returnGuild = new Guild
            {
                GuildId = guild.Id,
                Channels = withChannels ? GetGuildChannels(guild.Id, onlyId) : null
            };
            if (withOwner)
            {
                var member = members.First(p => p.User.Id == guild.Owner.Id);
                returnGuild.Owner = UserCreateHelper.GetMember(member.Id, onlyId);
            }
            if (!onlyId)
            {
                returnGuild.Name = guild.Name;
                returnGuild.Base64Avatar = guild.Info?.Avatar is not null ? Convert.ToBase64String(guild.Info.Avatar) : null;
            }
            return returnGuild;
        }

        public static Channel GetChannel(long channelId)
        {
            var dataContext = Program.DataContext;
            var channels = dataContext.Channels;
            channels.Include(p=>p.Guild).Load();
            var channel = channels.FirstOrDefault(p => p.Id == channelId);
            if (channel is null) return null;
            if (!ChannelChecks.IsChannelExists(channel.Id)) return null;
            var channel1 = new Channel
            {
                ChannelId = channel.Id,
                Guild = GetGuild(channel.Guild.Id, false, false, true),
                Name = channel.Name
            };
            return channel1;
        }

        public static GuildInvite GetGuildInvite(long inviteId)
        {
            var dataContext = Program.DataContext;
            var guildInvites = dataContext.GuildInvites;
            guildInvites.Include(p=>p.Guild).Load();
            guildInvites.Include(p=>p.CreatedByUser).Load();
            var invite = guildInvites.FirstOrDefault(p => p.Id == inviteId);
            if(invite is null) return null;
            return new GuildInvite
            {
                InviteId = invite.Id,
                CreatedAt = invite.CreatedAt,
                Guild = GetGuild(invite.Guild.Id, false, false, true),
                CreatedBy = UserCreateHelper.GetUser(invite.CreatedByUser.Id),
                InviteCode = invite.InviteCode,
                NumberOfUses = invite.NumberOfUses
            };
        }
    }
}