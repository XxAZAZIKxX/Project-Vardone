using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Base;
using VardoneLibrary.Exceptions;

namespace VardoneLibrary.Core
{
    public class VardoneClient : BaseClient
    {
        public VardoneClient(long userId, string token) : base(userId, token) { }

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
            var response = ExecutePostWithToken("users/getUser", null, new Dictionary<string, string> { { "secondId", id.ToString() } });
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
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetIncomingFriendRequests"),
                _ => JsonConvert.DeserializeObject<List<User>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<User> GetOutgoingFriendRequests()
        {
            var response = ExecutePostWithToken("users/getOutgoingFriendRequests");
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetOutgoingFriendRequests"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetOutgoingFriendRequests"),
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
                _ => JsonConvert.DeserializeObject<List<PrivateChat>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public PrivateChat GetPrivateChatWithUser(long userId)
        {
            var response = ExecutePostWithToken("chats/getPrivateChatWithUser", null, new Dictionary<string, string> { { "secondId", userId.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetPrivateChatWithUser"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateChatWithUser"),
                _ => JsonConvert.DeserializeObject<PrivateChat>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public List<PrivateMessage> GetPrivateMessagesFromChat(long chatId)
        {
            var response = ExecutePostWithToken("chats/getChatMessages", null,
                new Dictionary<string, string> { { "chatId", chatId.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetPrivateMessagesFromChat"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateMessagesFromChat"),
                _ => JsonConvert.DeserializeObject<List<PrivateMessage>>(JsonConvert.DeserializeObject<string>(response.Content))
            };
        }

        public void SendPrivateMessage(long userId, PrivateMessageModel message)
        {
            var response = ExecutePostWithToken("chats/sendChatMessage", JsonConvert.SerializeObject(message),
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception("BadRequest SendPrivateMessage");
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException("Unauthorized SendPrivateMessage");
                default: return;
            }
        }

        public void AddFriend(long idUser)
        {
            var response = ExecutePostWithToken("users/addFriend", null, new Dictionary<string, string> { { "secondId", idUser.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception("BadRequest AddFriend");
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException("Unauthorized AddFriend");
                default: return;
            }
        }

        public void DeleteFriend(long idUser)
        {
            var response = ExecutePostWithToken("users/deleteFriend", null, new Dictionary<string, string> { { "secondId", idUser.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception("BadRequest DeleteFriend");
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException("Unauthorized DeleteFriend");
                default: return;
            }
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

        public void DeleteMe()
        {
            var response = ExecutePostWithToken("users/deleteMe");
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception("BadRequest DeleteMe");
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException("Unauthorized DeleteMe");
                default:
                    {
                        UserId = long.MinValue;
                        Token = null;
                        return;
                    }
            }
        }

        public void UpdateUser(UpdateUserModel update)
        {
            var response = ExecutePostWithToken("users/updateUser", JsonConvert.SerializeObject(update));
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    throw new Exception("BadRequest UpdateUser");
                case HttpStatusCode.Unauthorized:
                    throw new UnauthorizedException("Unauthorized UpdateUser");
                default: return;
            }
        }
    }
}