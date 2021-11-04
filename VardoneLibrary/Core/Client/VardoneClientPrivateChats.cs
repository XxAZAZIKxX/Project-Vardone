using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Exceptions;
using static VardoneLibrary.VardoneEvents.VardoneEvents;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient
    {
        //Get
        /// <summary>
        /// Получить список приватных чатов текущего пользователя
        /// </summary>
        /// <returns></returns>
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
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<PrivateChat>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }

        /// <summary>
        /// Получить приватный чат с пользователем
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
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
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    onUpdateChatList?.Invoke();
                    return JsonConvert.DeserializeObject<PrivateChat>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }

        /// <summary>
        /// Получить список сообщений с приватного чата
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="limit">Количество сообщений</param>
        /// <param name="startFrom">Начинать с [n] id сообщения</param>
        /// <returns></returns>
        public List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, int limit = 0, long startFrom = 0)
        {
            var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null,
                new Dictionary<string, string>
                {
                    { "chatId", chatId.ToString() },
                    { "limit", limit.ToString() },
                    { "startFrom", startFrom.ToString() }
                });
            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (IsTokenExpired(response))
                    {
                        UpdateToken();
                        return GetPrivateMessagesFromChat(chatId, limit, startFrom);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<PrivateMessage>>(response.Content);
                default:
                    throw new Exception(response.Content);
            }
        }

        /// <summary>
        /// Получить список сообщений с приватного чата
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="read">Обновить ли счетчик прочитанных сообщений</param>
        /// <param name="limit">Количество сообщений для получения. По умолчанию 0 (получаются все)</param>
        /// <param name="startFrom">Начинать с [n] id сообщения</param>
        /// <returns></returns>
        internal List<PrivateMessage> GetPrivateMessagesFromChat(long chatId, bool read = true, int limit = 0,
            long startFrom = 0)
        {
            var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null,
                new Dictionary<string, string>
                {
                    { "chatId", chatId.ToString() },
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
                        return GetPrivateMessagesFromChat(chatId, read, limit, startFrom);
                    }
                    else
                        throw new UnauthorizedException();
                case HttpStatusCode.OK:
                    return JsonConvert.DeserializeObject<List<PrivateMessage>>((response.Content));
                default:
                    throw new Exception(response.Content);
            }
        }

        //Other
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
                    onUpdateChatList?.Invoke();
                    return;
                }
                default:
                    throw new Exception(response.Content);
            }
        }
    }
}