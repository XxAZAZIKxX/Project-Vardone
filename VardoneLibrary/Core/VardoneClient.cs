using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Base;
using VardoneLibrary.Exceptions;
using static VardoneLibrary.VardoneEvents.VardoneEvents;

namespace VardoneLibrary.Core
{
    public class VardoneClient : VardoneBaseClient
    {
        private VardoneClientBackground _clientBackground;
        public bool SetOnline => _clientBackground.setOnline;

        public VardoneClient(long userId, string token) : base(userId, token) =>
            _clientBackground = new VardoneClientBackground(this);
        private void StopClient()
        {
            _clientBackground.StopThreads();
            _clientBackground = null;
            UserId = long.MinValue;
            Token = null;
        }
        ~VardoneClient() => StopClient();

        //Get
        public User GetMe()
        {
            var response = ExecutePostWithToken("users/getMe");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetMe"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<User>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public User GetUser(long id)
        {
            var response = ExecutePostWithToken("users/getUser", null,
                new Dictionary<string, string> { { "secondId", id.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetUser"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<User>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public List<User> GetFriends()
        {
            var response = ExecutePostWithToken("users/getFriends");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetFriends"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public List<User> GetIncomingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getIncomingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetIncomingFriendRequests"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public List<User> GetOutgoingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getOutgoingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetOutgoingFriendRequests"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public List<PrivateChat> GetPrivateChats()
        {
            var response = ExecutePostWithToken("users/GetPrivateChats");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateChats"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<PrivateChat>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public PrivateChat GetPrivateChatWithUser(long userId)
        {
            var response = ExecutePostWithToken("chats/getPrivateChatWithUser", null,
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized GetPrivateChatWithUser");
                case HttpStatusCode.OK:
                    onUpdateChatList?.Invoke();
                    return JsonConvert.DeserializeObject<PrivateChat>(response.Content);
                default: throw new Exception(response.Content);
            }
        }

        public List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null,
                new Dictionary<string, string>
                {
                    {"chatId", chatId.ToString()}, {"limit", limit.ToString()}, {"startFrom", startFrom.ToString()}
                });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateMessagesFromChat"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<PrivateMessage>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        internal List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, bool read = true, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null,
                new Dictionary<string, string>
                {
                    {"chatId", chatId.ToString()},
                    {"limit", limit.ToString()},
                    {"startFrom", startFrom.ToString()},
                    {"read", read.ToString()}
                });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateMessagesFromChat"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<PrivateMessage>>((response.Content)),
                _ => throw new Exception(response.Content)
            };
        }

        public bool GetOnlineUser(long userId)
        {
            var response = ExecutePostWithToken("users/getUserOnline", null, new Dictionary<string, string> { { "secondId", userId.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetOnlineUser"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<bool>((response.Content)),
                _ => throw new Exception(response.Content)
            };
        }

        public List<Guild> GetGuilds()
        {
            var response = ExecutePostWithToken("users/getGuilds");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetGuilds"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<Guild>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public List<Channel> GetGuildChannels(long guildId)
        {
            var response = ExecutePostWithToken("guild/getGuildChannels", null, new Dictionary<string, string>
            {
                {"guildId", guildId.ToString()}
            });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetGuildChannels"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<Channel>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        public List<User> GetBannedGuildMembers(long guildId)
        {
            var response = ExecutePostWithToken("guilds/getBannedGuildMembers", null, new Dictionary<string, string>
            {
                {"guildId", guildId.ToString()}
            });
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetBannedGuildMembers"),
                HttpStatusCode.OK => JsonConvert.DeserializeObject<List<User>>(response.Content),
                _ => throw new Exception(response.Content)
            };
        }

        //Delete
        public void DeleteFriend(long idUser)
        {
            var response = ExecutePostWithToken("users/deleteFriend", null,
                new Dictionary<string, string> { { "secondId", idUser.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteFriend");
                case HttpStatusCode.OK:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke(true);
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }

        public void DeleteChat(long chatId)
        {
            var response = ExecutePostWithToken("chats/deletePrivateChat", null,
                new Dictionary<string, string> { { "chatId", chatId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteChat");
                case HttpStatusCode.OK:
                    {
                        onUpdateChatList?.Invoke();
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }

        public void DeleteMe()
        {
            var response = ExecutePostWithToken("users/deleteMe");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteMe");
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }

        //Update
        public void UpdateMe(UpdateUserModel update)
        {
            var response = ExecutePostWithToken("users/updateUser", JsonConvert.SerializeObject(update));
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdateMe");
                case HttpStatusCode.OK:
                    {
                        onUpdateUser?.Invoke(GetMe());
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }

        public void UpdatePassword(UpdatePasswordModel updatePassword)
        {
            var response = ExecutePostWithToken("users/updatePassword", JsonConvert.SerializeObject(updatePassword));
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdatePassword");
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }

        public void UpdateLastOnline()
        {
            var response = ExecutePostWithToken("users/setOnline");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdateLastOnline");
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }

        //Send
        public void SendPrivateMessage(long userId, MessageModel message)
        {
            var response = ExecutePostWithToken("chats/SendPrivateChatMessage", JsonConvert.SerializeObject(message),
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized SendPrivateMessage");
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }

        public void SendChannelMessage(long channelId, MessageModel message)
        {
            var response = ExecutePostWithToken("channels/sendChannelMessage", JsonConvert.SerializeObject(message), new Dictionary<string, string> { { "channelId", channelId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized SendChannelMessage");
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }

        //Close
        public void CloseCurrentSession()
        {
            var response = ExecutePostWithToken("users/closeCurrentSession");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized CloseCurrentSession");
            }
        }

        public void CloseAllSessions()
        {
            var response = ExecutePostWithToken("users/CloseAllSessions");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized CloseAllSessions");
            }
        }

        //Other
        public bool CheckFriend(long userId)
        {
            var response = ExecutePostWithToken("users/GetPrivateChats");
            return response.StatusCode switch
            {
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized CheckFriend"),
                HttpStatusCode.InternalServerError => throw new Exception(response.Content),
                _ => throw new Exception(response.Content)
            };
        }
        public void AddFriend(string secondUsername)
        {
            var response = ExecutePostWithToken("users/addFriend", null,
                new Dictionary<string, string> { { "secondUsername", secondUsername } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized AddFriend");
                case HttpStatusCode.OK:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke(true);
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }
    }
}