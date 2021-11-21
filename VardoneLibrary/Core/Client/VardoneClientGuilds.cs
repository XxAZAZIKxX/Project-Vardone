using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Exceptions;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient
    {
        //Get
        public List<Guild> GetGuilds()
        {
            var response = ExecutePostWithToken("users/getGuilds");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetGuilds();
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<Guild>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<Channel> GetGuildChannels(long guildId)
        {
            var response = ExecutePostWithToken("guilds/getGuildChannels", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetGuildChannels(guildId);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<Channel>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<BannedMember> GetBannedGuildMembers(long guildId)
        {
            var response = ExecutePostWithToken("guilds/getBannedGuildMembers", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetBannedGuildMembers(guildId);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<BannedMember>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<Member> GetGuildMembers(long guildId)
        {
            var response = ExecutePostWithToken("guilds/GetGuildMembers", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetGuildMembers(guildId);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<Member>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<GuildInvite> GetGuildInvites(long guildId)
        {
            var response = ExecutePostWithToken("guilds/getGuildInvites", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetGuildInvites(guildId);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<GuildInvite>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public List<ChannelMessage> GetChannelMessages(long channelId, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("channels/getChannelMessages", null,
                new Dictionary<string, string>
                {
                    { "channelId", channelId.ToString() },
                    { "limit", limit.ToString() },
                    { "startFrom", startFrom.ToString() }
                });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetChannelMessages(channelId, limit, startFrom);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<ChannelMessage>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        internal List<ChannelMessage> GetChannelMessages(long channelId, bool read = true, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("channels/getChannelMessages", null,
                new Dictionary<string, string>
                {
                    { "channelId", channelId.ToString() },
                    { "limit", limit.ToString() },
                    { "startFrom", startFrom.ToString() },
                    { "read", read.ToString() }
                });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetChannelMessages(channelId, read, limit, startFrom);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<ChannelMessage>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public DateTime? GetLastDeleteMessageTimeOnChannel(long channelId)
        {
            var response = ExecutePostWithToken("channels/getLastDeleteMessageTime", null,
                new Dictionary<string, string> { { "channelId", channelId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetLastDeleteMessageTimeOnChannel(channelId);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<DateTime?>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }

        //Create
        public void CreateGuild(string name = null)
        {
            var response = ExecutePostWithToken("guilds/createGuild", null,
                new Dictionary<string, string> { { "name", name } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        CreateGuild(name);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void CreateChannel(long guildId, string name = null)
        {
            var response = ExecutePostWithToken("channels/createChannel", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() }, { "name", name } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        CreateChannel(guildId, name);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public GuildInvite CreateGuildInvite(long guildId)
        {
            var response = ExecutePostWithToken("guilds/createGuildInvite", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return CreateGuildInvite(guildId);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<GuildInvite>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }

        //Delete
        public void DeleteGuild(long guildId)
        {
            var response = ExecutePostWithToken("guilds/deleteGuild", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteGuild(guildId);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void DeleteChannel(long channelId)
        {
            var response = ExecutePostWithToken("channels/deleteChannel", null,
                new Dictionary<string, string> { { "channelId", channelId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteChannel(channelId);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void DeleteGuildInvite(long inviteId)
        {
            var response = ExecutePostWithToken("guilds/deleteGuildInvite", null,
                new Dictionary<string, string> { { "inviteId", inviteId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteGuildInvite(inviteId);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void DeleteChannelMessage(long messageId)
        {
            var response = ExecutePostWithToken("channels/deleteChannelMessage", null,
                new Dictionary<string, string> { { "messageId", messageId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteChannelMessage(messageId);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }

        //Update
        public void UpdateGuild(UpdateGuildModel model)
        {
            var response = ExecutePostWithToken("guilds/updateGuild", JsonConvert.SerializeObject(model));
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        UpdateGuild(model);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void UpdateChannel(UpdateChannelModel model)
        {
            var response = ExecutePostWithToken("channels/updateChannel", JsonConvert.SerializeObject(model));
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        UpdateChannel(model);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }

        //Other
        public void SendChannelMessage(long channelId, MessageModel message)
        {
            var response = ExecutePostWithToken("channels/sendChannelMessage", JsonConvert.SerializeObject(message),
                new Dictionary<string, string> { { "channelId", channelId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        SendChannelMessage(channelId, message);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void JoinGuild(string inviteCode)
        {
            var response = ExecutePostWithToken("guilds/joinGuild", null,
                new Dictionary<string, string> { { "inviteCode", inviteCode } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        JoinGuild(inviteCode);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void LeaveGuild(long guildId)
        {
            var response = ExecutePostWithToken("guilds/leaveGuild", null,
                new Dictionary<string, string> { { "guildId", guildId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        LeaveGuild(guildId);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void KickGuildMember(long userId, long guildId)
        {
            var response = ExecutePostWithToken("guilds/kickGuildMember", null,
                new Dictionary<string, string>
                {
                    { "guildId", guildId.ToString() }, { "secondId", userId.ToString() }
                });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        KickGuildMember(userId, guildId);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void BanGuildMember(long userId, long guildId, string reason = null)
        {
            var response = ExecutePostWithToken("guilds/banGuildMember", null,
                new Dictionary<string, string>
                {
                    { "guildId", guildId.ToString() }, { "secondId", userId.ToString() }, { "reason", reason }
                });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        BanGuildMember(userId, guildId, reason);
                        break;
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return;
                default:
                    throw new Exception(response.Content);
            }
        }
        public void UnbanMember(long userId, long guildId)
        {
            var response = ExecutePostWithToken("guilds/unbanGuildMember", null, new Dictionary<string, string>
            {
                {"secondId", userId.ToString()},
                {"guildId", guildId.ToString()}
            });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        UnbanMember(userId, guildId);
                        return;
                    }
                    else throw new UnauthorizedException();
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
    }
}