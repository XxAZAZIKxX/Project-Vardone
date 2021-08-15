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
        public VardoneClient(long userId, string token) : base(userId, token) => _clientBackground = new VardoneClientBackground(this);

        public User GetMe()
        {
            var response = ExecutePostWithToken("users/getMe");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetMe"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetMe"),
                _ => JsonConvert.DeserializeObject<User>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public User GetUser(long id)
        {
            var response = ExecutePostWithToken("users/getUser", null,
                new Dictionary<string, string> { { "secondId", id.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetUser"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetUser"),
                _ => JsonConvert.DeserializeObject<User>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<User> GetFriends()
        {
            var response = ExecutePostWithToken("users/getFriends");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetFriends"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetFriends"),
                _ => JsonConvert.DeserializeObject<List<User>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<User> GetIncomingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getIncomingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetIncomingFriendRequests"),
                HttpStatusCode.Unauthorized =>
                    throw new UnauthorizedException("Unauthorized GetIncomingFriendRequests"),
                _ => JsonConvert.DeserializeObject<List<User>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<User> GetOutgoingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getOutgoingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetOutgoingFriendRequests"),
                HttpStatusCode.Unauthorized =>
                    throw new UnauthorizedException("Unauthorized GetOutgoingFriendRequests"),
                _ => JsonConvert.DeserializeObject<List<User>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<PrivateChat> GetPrivateChats()
        {
            var response = ExecutePostWithToken("users/GetPrivateChats");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetPrivateChats"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateChats"),
                _ => JsonConvert.DeserializeObject<List<PrivateChat>>(
                    JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public PrivateChat GetPrivateChatWithUser(long userId)
        {
            var response = ExecutePostWithToken("chats/getPrivateChatWithUser", null, new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest GetPrivateChatWithUser");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized GetPrivateChatWithUser");
                default:
                    onUpdateChatList?.Invoke();
                    return JsonConvert.DeserializeObject<PrivateChat>(JsonConvert.DeserializeObject<string>(response.Content));
            }
        }

        public List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null, new Dictionary<string, string>
                {
                    {"chatId", chatId.ToString()}, {"limit", limit.ToString()}, {"startFrom", startFrom.ToString()}
                });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetPrivateMessagesFromChat"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateMessagesFromChat"),
                _ => JsonConvert.DeserializeObject<List<PrivateMessage>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        internal List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, bool read = true, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null, new Dictionary<string, string>
            {
                {"chatId", chatId.ToString()}, {"limit", limit.ToString()}, {"startFrom", startFrom.ToString()}, {"read", read.ToString()}
            });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetPrivateMessagesFromChat"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateMessagesFromChat"),
                _ => JsonConvert.DeserializeObject<List<PrivateMessage>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public bool CheckFriend(long userId)
        {
            var response = ExecutePostWithToken("users/GetPrivateChats");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest CheckFriend"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized CheckFriend"),
                _ => JsonConvert.DeserializeObject<bool>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public bool GetOnlineUser(long userId)
        {
            var response = ExecutePostWithToken("users/getUserOnline", null,
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetOnlineUser"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetOnlineUser"),
                _ => JsonConvert.DeserializeObject<bool>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public void SendPrivateMessage(long userId, MessageModel message)
        {
            var response = ExecutePostWithToken("chats/SendPrivateChatMessage", JsonConvert.SerializeObject(message),
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest SendPrivateMessage");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized SendPrivateMessage");
                default: return;
            }
        }

        public void AddFriend(string secondUsername)
        {
            var response = ExecutePostWithToken("users/addFriend", null, new Dictionary<string, string> { { "secondUsername", secondUsername } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest AddFriend");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized AddFriend");
                default:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke(true);
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
            }
        }

        public void DeleteFriend(long idUser)
        {
            var response = ExecutePostWithToken("users/deleteFriend", null,
                new Dictionary<string, string> { { "secondId", idUser.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest DeleteFriend");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteFriend");
                default:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke(true);
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
            }
        }

        public void DeleteChat(long chatId)
        {
            var response = ExecutePostWithToken("chats/deletePrivateChat", null, new Dictionary<string, string> { { "chatId", chatId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest DeleteChat");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteChat");
                default:
                    {
                        onUpdateChatList?.Invoke();
                        return;
                    }
            }
        }

        public void DeleteMe()
        {
            var response = ExecutePostWithToken("users/deleteMe");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest DeleteMe");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteMe");
            }
        }

        public void UpdateMe(UpdateUserModel update)
        {
            var response = ExecutePostWithToken("users/updateUser", JsonConvert.SerializeObject(update));
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest UpdateMe");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdateMe");
                default:
                    {
                        onUpdateUser?.Invoke(GetMe());
                        return;
                    }
            }
        }

        public void UpdatePassword(UpdatePasswordModel updatePassword)
        {
            var response = ExecutePostWithToken("users/updatePassword", JsonConvert.SerializeObject(updatePassword));
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest UpdatePassword");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdatePassword");
                default: return;
            }
        }

        public void UpdateLastOnline()
        {
            var response = ExecutePostWithToken("users/setOnline");
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest Exception");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdateLastOnline");
                default: return;
            }
        }

        public void CloseCurrentSession()
        {
            var response = ExecutePostWithToken("users/closeCurrentSession");
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest CloseCurrentSession");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized CloseCurrentSession");
            }
            StopClient();
        }

        public void CloseAllSessions()
        {
            var response = ExecutePostWithToken("users/CloseAllSessions");
            StopClient();
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest CloseAllSessions");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized CloseAllSessions");
            }
        }

        private void StopClient()
        {
            _clientBackground.StopThreads();
            _clientBackground = null;
            UserId = long.MinValue;
            Token = null;
        }
        ~VardoneClient() => StopClient();
    }
}