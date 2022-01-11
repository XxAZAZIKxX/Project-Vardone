using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Exceptions;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient
    {
        //===============================[GET]===============================
        /// <summary>
        /// Получить список приватных чатов текущего пользователя
        /// </summary>
        /// <returns></returns>
        public List<PrivateChat> GetPrivateChats() => GetPrivateChats(false);
        internal List<PrivateChat> GetPrivateChats(bool onlyId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/GetPrivateChats", onlyId:onlyId);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        if (IsTokenExpired(response))
                        {
                            UpdateToken();
                            continue;
                        }
                        else
                            throw new UnauthorizedException();
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<List<PrivateChat>>(response.Content);
                    default:
                        throw new Exception(response.Content);
                }
            }
        }

        /// <summary>
        /// Получить приватный чат с пользователем
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        public PrivateChat GetPrivateChatWithUser(long userId) => GetPrivateChatWithUser(userId, false);
        internal PrivateChat GetPrivateChatWithUser(long userId, bool onlyId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/getPrivateChatWithUser", null,
                    new Dictionary<string, string> { { "secondId", userId.ToString() } }, onlyId: onlyId);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        if (IsTokenExpired(response))
                        {
                            UpdateToken();
                            continue;
                        }
                        else
                            throw new UnauthorizedException();
                    case HttpStatusCode.OK:
                        UpdateChatList();
                        return JsonConvert.DeserializeObject<PrivateChat>(response.Content);
                    default:
                        throw new Exception(response.Content);
                }
            }
        }

        /// <summary>
        /// Получить список сообщений с приватного чата
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="limit">Количество сообщений</param>
        /// <param name="startFrom">Начинать с [n] id сообщения</param>
        /// <returns></returns>
        public List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, int limit = 0, long startFrom = 0) => GetPrivateMessagesFromChat(chatId, limit, startFrom, false);
        internal List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, int limit, long startFrom, bool onlyId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null, new Dictionary<string, string> { { "chatId", chatId.ToString() }, { "limit", limit.ToString() }, { "startFrom", startFrom.ToString() } }, onlyId: onlyId);
                switch (response.StatusCode)
                {
                    case HttpStatusCode.Unauthorized:
                        if (IsTokenExpired(response))
                        {
                            UpdateToken();
                            continue;
                        }
                        else
                            throw new UnauthorizedException();
                    case HttpStatusCode.OK:
                        return JsonConvert.DeserializeObject<List<PrivateMessage>>((response.Content));
                    default:
                        throw new Exception(response.Content);
                }
            }
        }

        public DateTime? GetLastDeleteTimeOnChat(long chatId)
        {
            var response = ExecutePostWithToken("chats/getLastDeleteMessageTime", null, new Dictionary<string, string>
            {
                { "chatId", chatId.ToString() }
            });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetLastDeleteTimeOnChat(chatId);
                    }
                    else throw new UnauthorizedException();
                case HttpStatusCode.OK: return JsonConvert.DeserializeObject<DateTime?>(response.Content);
                default: throw new Exception(response.Content);
            }
        }

        //===============================[OTHER]===============================
        /// <summary>
        /// Удалить сообщение
        /// </summary>
        /// <param name="messageId">Id сообщения</param>
        public void DeletePrivateMessage(long messageId)
        {
            var response = ExecutePostWithToken("chats/deletePrivateChatMessage", null,
                new Dictionary<string, string> { { "messageId", messageId.ToString() } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        DeletePrivateMessage(messageId);
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

        /// <summary>
        /// Отправить сообщение пользователю
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="message">Сообщение</param>
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
                        SendPrivateMessage(userId, message);
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

        /// <summary>
        /// Удалить чат
        /// </summary>
        /// <param name="chatId">Id чата</param>
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
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    {
                        UpdateChatList();
                        return;
                    }
                default:
                    throw new Exception(response.Content);
            }
        }
    }
}