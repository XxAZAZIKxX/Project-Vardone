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
    public static class GuildCreateHelper
    {
        public static List<Channel> GetGuildChannels(long guildId)
        {
            if (!GuildChecks.IsGuildExists(guildId)) return null;
            
            var channels = Program.DataContext.Channels;
            channels.Include(p=>p.Guild).Load();
            channels.Include(p=>p.Guild.Info).Load();

            var channelsList = new List<Channel>();

            foreach (var channel in channels.Where(p=>p.Guild.Id == guildId))
            {
                channelsList.Add(new Channel
                {
                    ChannelId = channel.Id,
                    Name = channel.Name,
                    Guild = new Guild
                    {
                        GuildId = channel.Guild.Id,
                        Name = channel.Guild.Name,
                        Base64Avatar = channel.Guild.Info?.Avatar is not null ? Convert.ToBase64String(channel.Guild.Info.Avatar) : null
                    }
                });
            }
            return channelsList;
        }

        public static Guild GetGuild(GuildsTable guild)
        {
            var members = Program.DataContext.GuildMembers;
            members.Include(p=>p.Guild).Load();
            members.Include(p=>p.User).Load();
            var member = members.First(p => p.User.Id == guild.Owner.Id);
            var returnGuild = new Guild
            {
                GuildId = guild.Id,
                Owner = new Member
                {
                    NumberInvitedMembers = member.NumberOfInvitedMembers,
                    JoinDate = member.JoinDate,
                    User = UserCreateHelper.GetUser(guild.Owner)
                },
                Channels = GetGuildChannels(guild.Id),
                Name = guild.Name,
                Base64Avatar = guild.Info?.Avatar is not null ? Convert.ToBase64String(guild.Info.Avatar) : null
            };
            return returnGuild;
        }

        public static Channel GetChannel(ChannelsTable channel)
        {
            return new Channel
            {
                ChannelId = channel.Id,
                Name = channel.Name,
                Guild = GetGuild(channel.Guild)
            };
        }
    }
}