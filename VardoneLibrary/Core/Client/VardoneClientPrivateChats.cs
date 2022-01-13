using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Models.GeneralModels.Users;

namespace VardoneLibrary.Core.Client
{
    public partial class VardoneClient
    {
        //===============================[GET]===============================
        /// <summary>
        /// Получить список приватных чатов текущего пользователя
        /// </summary>
        /// <returns></returns>
        public PrivateChat[] GetPrivateChats() => GetPrivateChats(false);
        internal PrivateChat[] GetPrivateChats(bool onlyId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("users/GetPrivateChats", onlyId: onlyId);
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<PrivateChat[]>(response.Content);
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
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<PrivateChat>(response.Content);
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

        /// <summary>
        /// Получить список сообщений с приватного чата
        /// </summary>
        /// <param name="chatId">Id чата</param>
        /// <param name="limit">Количество сообщений</param>
        /// <param name="startFrom">Начинать с [n] id сообщения</param>
        /// <returns></returns>
        public PrivateMessage[] GetPrivateMessagesFromChat(long chatId, int limit = 0, long startFrom = 0) => GetPrivateMessagesFromChat(chatId, limit, startFrom, false);
        internal PrivateMessage[] GetPrivateMessagesFromChat(long chatId, int limit, long startFrom, bool onlyId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/GetPrivateChatMessages", null, new Dictionary<string, string> { { "chatId", chatId.ToString() }, { "limit", limit.ToString() }, { "startFrom", startFrom.ToString() } }, onlyId: onlyId);
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<PrivateMessage[]>(response.Content);
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

        public PrivateMessage GetPrivateMessage(long messageId) => GetPrivateMessage(messageId, false);
        internal PrivateMessage GetPrivateMessage(long messageId, bool onlyId)
        {
            while (true)
            {
                var response = ExecutePostWithToken(@"chats/getPrivateMessage",
                    queryParameters: new Dictionary<string, string>
                    {
                        { "messageId", messageId.ToString() }
                    }, onlyId: onlyId);
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<PrivateMessage>(response.Content);
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

        public DateTime? GetLastDeleteTimeOnChat(long chatId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/getLastDeleteMessageTime", null, new Dictionary<string, string> { { "chatId", chatId.ToString() } });
                switch (ResponseHandler.GetResponseStatus(response))
                {
                    case ResponseStatus.Ok:
                        return JsonConvert.DeserializeObject<DateTime?>(response.Content);
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

        //===============================[OTHER]===============================
        /// <summary>
        /// Удалить сообщение
        /// </summary>
        /// <param name="messageId">Id сообщения</param>
        public void DeletePrivateMessage(long messageId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/deletePrivateChatMessage", null, new Dictionary<string, string> { { "messageId", messageId.ToString() } });
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

        /// <summary>
        /// Отправить сообщение пользователю
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="message">Сообщение</param>
        public void SendPrivateMessage(long userId, MessageModel message)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/SendPrivateChatMessage", JsonConvert.SerializeObject(message), new Dictionary<string, string> { { "secondId", userId.ToString() } });
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

        /// <summary>
        /// Удалить чат
        /// </summary>
        /// <param name="chatId">Id чата</param>
        public void DeleteChat(long chatId)
        {
            while (true)
            {
                var response = ExecutePostWithToken("chats/deletePrivateChat", null, new Dictionary<string, string> { { "chatId", chatId.ToString() } });
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