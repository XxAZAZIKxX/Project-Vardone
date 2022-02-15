using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.GeneralModels.Guilds;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient
    {
        //===============================[GET]===============================
        public Guild GetGuild(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken(@"guilds/getGuild", queryParameters: new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<Guild>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Guild[] GetGuilds()
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/getGuilds");
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<Guild[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Channel[] GetGuildChannels(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/getGuildChannels", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<Channel[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Channel GetGuildChannel(long channelId)
        {
            while (true)
            {
                var response = ExecutePostWithToken(@"channels/getChannel",
                    queryParameters: new Dictionary<string, string>
                    {
                        { "channelId", channelId.ToString() }
                    });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<Channel>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }


        public BannedMember[] GetBannedGuildMembers(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/getBannedGuildMembers", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<BannedMember[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public Member[] GetGuildMembers(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/GetGuildMembers", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<Member[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public GuildInvite[] GetGuildInvites(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/getGuildInvites", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<GuildInvite[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ChannelMessage[] GetChannelMessages(long channelId, int limit=0, long startFrom=0)
        {
            while (true)
            {
                var response = ExecutePostWithToken("channels/getChannelMessages", null, new Dictionary<string, string> { { "channelId", channelId.ToString() }, { "limit", limit.ToString() }, { "startFrom", startFrom.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<ChannelMessage[]>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public ChannelMessage GetChannelMessage(long messageId)
        {
            while (true)
            {
                var response = ExecutePostWithToken(@"channels/getChannelMessage", queryParameters: new Dictionary<string, string> {{"messageId", messageId.ToString()}});
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<ChannelMessage>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.Content);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[CREATE]===============================
        public void CreateGuild(string name = null)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/createGuild", null, new Dictionary<string, string> { { "name", name } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void CreateChannel(long guildId, string name = null)
        {
            while (true)
            {
                var response = ExecutePostWithToken("channels/createChannel", null, new Dictionary<string, string> { { "guildId", guildId.ToString() }, { "name", name } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public GuildInvite CreateGuildInvite(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/createGuildInvite", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<GuildInvite>(response.Content);
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return null;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[DELETE]===============================
        public void DeleteGuild(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/deleteGuild", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void DeleteChannel(long channelId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("channels/deleteChannel", null, new Dictionary<string, string> { { "channelId", channelId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void DeleteGuildInvite(long inviteId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/deleteGuildInvite", null, new Dictionary<string, string> { { "inviteId", inviteId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void DeleteChannelMessage(long messageId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("channels/deleteChannelMessage", null, new Dictionary<string, string> { { "messageId", messageId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[UPDATE]===============================
        public void UpdateGuild(UpdateGuildModel model)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/updateGuild", JsonConvert.SerializeObject(model));
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void UpdateChannel(UpdateChannelModel model)
        {
            while (true)
            {
                var response = ExecutePostWithToken("channels/updateChannel", JsonConvert.SerializeObject(model));
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        //===============================[OTHER]===============================
        public void SendChannelMessage(long channelId, MessageModel message)
        {
            while (true)
            {
                var response = ExecutePostWithToken("channels/sendChannelMessage", JsonConvert.SerializeObject(message), new Dictionary<string, string> { { "channelId", channelId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void JoinGuild(string inviteCode)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/joinGuild", null, new Dictionary<string, string> { { "inviteCode", inviteCode } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void LeaveGuild(long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/leaveGuild", null, new Dictionary<string, string> { { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void KickGuildMember(long userId, long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/kickGuildMember", null, new Dictionary<string, string> { { "guildId", guildId.ToString() }, { "secondId", userId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void BanGuildMember(long userId, long guildId, string reason = null)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/banGuildMember", null, new Dictionary<string, string> { { "guildId", guildId.ToString() }, { "secondId", userId.ToString() }, { "reason", reason } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void UnbanMember(long userId, long guildId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("guilds/unbanGuildMember", null, new Dictionary<string, string> { { "secondId", userId.ToString() }, { "guildId", guildId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return;
                    case ResponseStatus.UpdateToken:
                        UpdateToken();
                        continue;
                    case ResponseStatus.InvalidToken:
                        EventDisconnectInvoke();
                        return;
                    case ResponseStatus.Error:
                        throw new Exception(response.ErrorMessage);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}