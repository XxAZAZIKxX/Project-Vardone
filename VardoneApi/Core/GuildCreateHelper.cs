using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VardoneApi.Core.Checks;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Guild;

namespace VardoneApi.Core
{
    public abstract class GuildCreateHelper
    {
        public static List<Channel> GetGuildChannels(long guildId)
        {
            if (!GuildChecks.IsGuildExists(guildId)) return null;

            var dataContext = Program.DataContext;
            var channels = dataContext.Channels;
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
    }
}