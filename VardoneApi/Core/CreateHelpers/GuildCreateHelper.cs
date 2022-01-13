using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.Checks;
using VardoneApi.Entity.Models.Channels;
using VardoneApi.Entity.Models.Guilds;
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
                channelsList.Add(GetChannel(channel, onlyId));
            }
            return channelsList.ToArray();
        }

        public static Guild GetGuild(GuildsTable guild, bool withChannels = true, bool withOwner = true, bool onlyId = false)
        {
            if (guild is null) return null;
            if (!GuildChecks.IsGuildExists(guild.Id)) return null;
            var members = Program.DataContext.GuildMembers;
            members.Include(p => p.Guild).Load();
            members.Include(p => p.Guild.Owner).Load();
            members.Include(p => p.User).Load();

            var returnGuild = new Guild
            {
                GuildId = guild.Id,
                Channels = withChannels ? GetGuildChannels(guild.Id, onlyId) : null
            };
            if (withOwner)
            {
                var member = members.First(p => p.User.Id == guild.Owner.Id);
                returnGuild.Owner = UserCreateHelper.GetMember(member, onlyId);
            }
            if (!onlyId)
            {
                returnGuild.Name = guild.Name;
                returnGuild.Base64Avatar = guild.Info?.Avatar is not null ? Convert.ToBase64String(guild.Info.Avatar) : null;
            }
            return returnGuild;
        }

        public static Channel GetChannel(ChannelsTable channel, bool onlyId = false)
        {
            if (channel is null) return null;
            if (!ChannelChecks.IsChannelExists(channel.Id)) return null;
            var channel1 = new Channel
            {
                ChannelId = channel.Id,
                Guild = GetGuild(channel.Guild, false, false, true)
            };
            if (!onlyId)
            {
                channel1.Name = channel.Name;
            }
            return channel1;
        }
    }
}