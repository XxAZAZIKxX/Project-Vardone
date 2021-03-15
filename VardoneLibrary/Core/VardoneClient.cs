using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core.Base;
using VardoneLibrary.Exceptions;
using static VardoneLibrary.VardoneEvents.VardoneEvents;

namespace VardoneLibrary.Core
{
    public class VardoneClient : VardoneBaseClient
    {
        /// <summary>
        /// Поток проверки наличия новых сообщений
        /// </summary>
        private Thread _checkingMessageThread;
        private bool _isCheckMessageThreadWork = true;
        /// <summary>
        /// Поток который обновляет онлайн текущего пользователя
        /// </summary>
        private Thread _settingOnlineThread;
        private bool _isSettingOnlineThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновления списка друзей
        /// </summary>
        private Thread _checkingUpdatesOnFriendListThread;
        private bool _isCheckingUpdatesOnFriendListThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновления списка чатов
        /// </summary>
        private Thread _checkingUpdatesOnChatListThread;
        private bool _isCheckingUpdatesOnChatListThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновление статуса друзей
        /// </summary>
        private Thread _checkOnlineUsersThread;
        private bool _isCheckOnlineUsersThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновление списка входящих запросов в друзья
        /// </summary>
        private Thread _checkUpdatesIncomingRequestThread;
        private bool _isCheckUpdatesIncomingRequestThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновление списка исходящих запросов в друзья
        /// </summary>
        private Thread _checkUpdatesOutgoingRequestThread;
        private bool _isCheckUpdatesOutgoingRequestThreadWork = true;
        /// <summary>
        /// Обновлять ли статус текущего пользователя
        /// </summary>
        public bool setOnline = true;
        public VardoneClient(long userId, string token) : base(userId, token) => SetThreads();

        private void SetThreads()
        {
            _checkingMessageThread = new Thread(() =>
            {
                var dictionary = new Dictionary<long, long>();
                foreach (var chat in GetPrivateChats())
                {
                    var privateMessages = GetPrivateMessagesFromChat(chat.ChatId, 1).OrderByDescending(p => p.MessageId).ToList();
                    if (privateMessages.Count == 0)
                    {
                        dictionary.Add(chat.ChatId, -1);
                        continue;
                    }

                    var message = privateMessages[0];
                    if (message is null) continue;
                    dictionary.Add(chat.ChatId, message.MessageId);
                }

                try
                {
                    while (_isCheckMessageThreadWork)
                    {
                        foreach (var chat in GetPrivateChats())
                        {
                            var privateMessages = GetPrivateMessagesFromChat(chat.ChatId, 1).OrderByDescending(p => p.MessageId).ToList();
                            if (privateMessages.Count == 0) continue;
                            var message = privateMessages[0];
                            if (message is null) continue;
                            if (!dictionary.ContainsKey(chat.ChatId))
                            {
                                dictionary.Add(chat.ChatId, message.MessageId);
                                onNewPrivateMessage?.Invoke(message);
                            }

                            if (dictionary[chat.ChatId] == message.MessageId) continue;
                            dictionary[chat.ChatId] = message.MessageId;
                            onNewPrivateMessage?.Invoke(message);
                        }

                        Thread.Sleep(200);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isCheckMessageThreadWork = false;
                    else throw;
                }
            });
            _checkingMessageThread.Start();
            //
            _settingOnlineThread = new Thread(() =>
            {
                try
                {
                    while (_isSettingOnlineThreadWork)
                    {
                        if (setOnline) UpdateLastOnline();
                        Thread.Sleep(4 * 60 * 1000);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isSettingOnlineThreadWork = false;
                    else throw;
                }
            });
            _settingOnlineThread.Start();
            //
            _checkingUpdatesOnFriendListThread = new Thread(() =>
            {
                var list = GetFriends();
                try
                {
                    while (_isCheckingUpdatesOnFriendListThreadWork)
                    {
                        var friends = GetFriends();
                        if (list.Count != friends.Count)
                        {
                            onUpdateFriendList?.Invoke();
                            list = friends;
                            continue;
                        }

                        var list1 = list;
                        if (friends.Any(user => !list1.Contains(user))) onUpdateFriendList?.Invoke();
                        if (list1.Any(user => !friends.Contains(user))) onUpdateFriendList?.Invoke();
                        list = friends;
                        Thread.Sleep(10 * 1000);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isCheckingUpdatesOnFriendListThreadWork = false;
                    else throw;
                }
            });
            _checkingUpdatesOnFriendListThread.Start();
            //
            _checkingUpdatesOnChatListThread = new Thread(() =>
            {
                var list = GetPrivateChats();
                try
                {
                    while (_isCheckingUpdatesOnChatListThreadWork)
                    {
                        foreach (var _ in GetPrivateChats().Where(privateChat => !list.Contains(privateChat)))
                            onUpdateChatList?.Invoke();
                        Thread.Sleep(10 * 1000);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isCheckingUpdatesOnChatListThreadWork = false;
                    else throw;
                }
            });
            _checkingUpdatesOnChatListThread.Start();
            //
            _checkOnlineUsersThread = new Thread(() =>
            {
                var dict = GetFriends().ToDictionary(friend => friend.UserId, friend => GetOnlineUser(friend.UserId));
                try
                {
                    while (_isCheckOnlineUsersThreadWork)
                    {
                        foreach (var user in GetFriends())
                        {
                            var onlineUser = GetOnlineUser(user.UserId);
                            if (!dict.ContainsKey(user.UserId))
                            {
                                dict.Add(user.UserId, onlineUser);
                                continue;
                            }

                            if (dict[user.UserId] != onlineUser) onUpdateOnline.Invoke(user);
                            dict[user.UserId] = onlineUser;
                        }

                        Thread.Sleep(10 * 1000);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isCheckOnlineUsersThreadWork = false;
                    else throw;
                }
            });
            _checkOnlineUsersThread.Start();
            //
            _checkUpdatesIncomingRequestThread = new Thread(() =>
            {
                var list = GetIncomingFriendRequests();
                try
                {
                    while (_isCheckUpdatesIncomingRequestThreadWork)
                    {
                        var incomingFriends = GetIncomingFriendRequests();
                        if (list.Count != incomingFriends.Count)
                        {
                            onUpdateIncomingFriendRequestList?.Invoke();
                            list = incomingFriends;
                            continue;
                        }

                        var list1 = list;
                        if (incomingFriends.Any(user => !list1.Contains(user))) onUpdateIncomingFriendRequestList?.Invoke();
                        else if (list1.Any(user => !incomingFriends.Contains(user))) onUpdateIncomingFriendRequestList?.Invoke();
                        list = incomingFriends;
                        Thread.Sleep(10 * 1000);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isCheckUpdatesIncomingRequestThreadWork = false;
                    else throw;
                }
            });
            _checkUpdatesIncomingRequestThread.Start();
            //
            _checkUpdatesOutgoingRequestThread = new Thread(() =>
            {
                var list = GetOutgoingFriendRequests();
                try
                {
                    while (_isCheckUpdatesOutgoingRequestThreadWork)
                    {
                        var outgoingFriends = GetOutgoingFriendRequests();
                        if (list.Count != outgoingFriends.Count)
                        {
                            onUpdateFriendList?.Invoke();
                            list = outgoingFriends;
                            continue;
                        }

                        var list1 = list;
                        if (outgoingFriends.Any(user => !list1.Contains(user))) onUpdateOutgoingFriendRequestList?.Invoke();
                        else if (list1.Any(user => !outgoingFriends.Contains(user))) onUpdateOutgoingFriendRequestList?.Invoke();
                        list = outgoingFriends;
                        Thread.Sleep(10 * 1000);
                    }
                }
                catch (Exception e)
                {
                    if (e is UnauthorizedException) _isCheckUpdatesOutgoingRequestThreadWork = false;
                    else throw;
                }
            });
            _checkUpdatesOutgoingRequestThread.Start();
        }

        private void StopThreads()
        {
            _isCheckMessageThreadWork = _isSettingOnlineThreadWork =
                _isCheckingUpdatesOnFriendListThreadWork = _isCheckingUpdatesOnChatListThreadWork =
                    _isCheckOnlineUsersThreadWork = _isCheckUpdatesIncomingRequestThreadWork =
                        _isCheckUpdatesOutgoingRequestThreadWork = false;
        }

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
            var response = ExecutePostWithToken("chats/getPrivateChatWithUser", null,
                new Dictionary<string, string> { { "secondId", userId.ToString() } });
            return response.StatusCode switch
            {
                HttpStatusCode.BadRequest => throw new Exception("BadRequest GetPrivateChatWithUser"),
                HttpStatusCode.Unauthorized => throw new UnauthorizedException("Unauthorized GetPrivateChatWithUser"),
                _ => JsonConvert.DeserializeObject<PrivateChat>(JsonConvert.DeserializeObject<string>(response.Content))
            };
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

        public void SendPrivateMessage(long userId, PrivateMessageModel message)
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
            var response = ExecutePostWithToken("users/addFriend", null,
                new Dictionary<string, string> { { "secondUsername", secondUsername } });
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest AddFriend");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized AddFriend");
                default:
                    {
                        onUpdateFriendList?.Invoke();
                        onUpdateIncomingFriendRequestList?.Invoke();
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
                        onUpdateIncomingFriendRequestList?.Invoke();
                        onUpdateOutgoingFriendRequestList?.Invoke();
                        return;
                    }
            }
        }

        public void DeleteMe()
        {
            StopThreads();
            var response = ExecutePostWithToken("users/deleteMe");
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest DeleteMe");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized DeleteMe");
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
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest UpdateUser");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdateUser");
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
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest UpdateUser");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized UpdateUser");
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
            StopThreads();
            var response = ExecutePostWithToken("users/closeCurrentSession");
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest CloseCurrentSession");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized CloseCurrentSession");
                default: return;
            }
        }

        public void CloseAllSessions()
        {
            StopThreads();
            var response = ExecutePostWithToken("users/CloseAllSessions");
            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest: throw new Exception("BadRequest CloseAllSessions");
                case HttpStatusCode.Unauthorized: throw new UnauthorizedException("Unauthorized CloseAllSessions");
                default: return;
            }
        }
    }
}