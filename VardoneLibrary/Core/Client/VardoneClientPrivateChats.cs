﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Exceptions;
using static VardoneLibrary.VardoneEvents.VardoneEvents;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient
    {
        //Get
        public List<PrivateChat> GetPrivateChats()
        {
            var response = ExecutePostWithToken("users/GetPrivateChats");
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetPrivateChats();
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<PrivateChat>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }
        public PrivateChat GetPrivateChatWithUser(long userId)
        {
            var response = ExecutePostWithToken("chats/getPrivateChatWithUser", null,
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetPrivateChatWithUser(userId);
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
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
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetPrivateMessagesFromChat(chatId,limit,startFrom);
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<PrivateMessage>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
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
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetPrivateMessagesFromChat(chatId,read,limit,startFrom);
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<PrivateMessage>>((response.Content));
                default:
                    throw new Exception(response.Content);
            }
        }
        //Other
        public void DeletePrivateMessage(long messageId)
        {
            var response = ExecutePostWithToken("chats/deletePrivateChatMessage", null, new Dictionary<string, string>
            {
                {"messageId", messageId.ToString()}
            });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeletePrivateMessage(messageId);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
        public void SendPrivateMessage(long userId, MessageModel message)
        {
            var response = ExecutePostWithToken("chats/SendPrivateChatMessage", JsonConvert.SerializeObject(message),
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        SendPrivateMessage(userId,message);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK: return;
                default: throw new Exception(response.Content);
            }
        }
        public void DeleteChat(long chatId)
        {
            var response = ExecutePostWithToken("chats/deletePrivateChat", null,
                new Dictionary<string, string> { { "chatId", chatId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeleteChat(chatId);
                        break;
                    }
                    else if (IsTokenInvalid(response)) throw new UnauthorizedException();
                    else goto default;
                case HttpStatusCode.OK:
                    {
                        onUpdateChatList?.Invoke();
                        return;
                    }
                default: throw new Exception(response.Content);
            }
        }
    }
}