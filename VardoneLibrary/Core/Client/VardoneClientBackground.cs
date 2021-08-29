using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VardoneLibrary.Exceptions;
using static VardoneLibrary.VardoneEvents.VardoneEvents;

namespace VardoneLibrary.Core.Client
{
    internal class VardoneClientBackground
    {
        private VardoneClient _client;

        /// <summary>
        /// Поток проверки наличия новых сообщений
        /// </summary>
        private Thread _checkingPrivateMessageThread;
        private bool _isCheckPrivateMessageThreadWork = true;
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
        /// Поток проверяющий на обновление списка серверов
        /// </summary>
        private Thread _checkUpdatesOnGuildListThread;
        private bool _isCheckUpdatesOnGuildListThreadWork = true;
        /// <summary>
        /// Обновлять ли статус текущего пользователя
        /// </summary>
        internal bool setOnline = true;

        public VardoneClientBackground(VardoneClient client)
        {
            _client = client;
            SetThreads();
        }
        ~VardoneClientBackground()
        {
            StopThreads();
            _client = null;
        }

        private void SetThreads()
        {
            _checkingPrivateMessageThread = new Thread(CheckingPrivateMessageThread);
            _checkingPrivateMessageThread.Start();
            //
            _settingOnlineThread = new Thread(SettingOnlineThread);
            _settingOnlineThread.Start();
            //
            _checkingUpdatesOnFriendListThread = new Thread(CheckingUpdatesOnFriendListThread);
            _checkingUpdatesOnFriendListThread.Start();
            //
            _checkingUpdatesOnChatListThread = new Thread(CheckingUpdatesOnChatListThread);
            _checkingUpdatesOnChatListThread.Start();
            //
            _checkOnlineUsersThread = new Thread(CheckOnlineUsersThread);
            _checkOnlineUsersThread.Start();
            //
            _checkUpdatesIncomingRequestThread = new Thread(CheckUpdatesIncomingRequestThread);
            _checkUpdatesIncomingRequestThread.Start();
            //
            _checkUpdatesOutgoingRequestThread = new Thread(CheckUpdatesOutgoingRequestThread);
            _checkUpdatesOutgoingRequestThread.Start();
            //
            _checkUpdatesOnGuildListThread = new Thread(CheckUpdateOnGuildListThread);
            _checkUpdatesOnGuildListThread.Start();
        }
        internal void StopThreads()
        {
            _isCheckPrivateMessageThreadWork = _isSettingOnlineThreadWork =
                _isCheckingUpdatesOnFriendListThreadWork = _isCheckingUpdatesOnChatListThreadWork =
                    _isCheckOnlineUsersThreadWork = _isCheckUpdatesIncomingRequestThreadWork =
                        _isCheckUpdatesOutgoingRequestThreadWork = _isCheckUpdatesOnGuildListThreadWork = false;
        }

        private void CheckUpdatesOutgoingRequestThread()
        {
            var list = _client.GetOutgoingFriendRequests();
            try
            {
                while (_isCheckUpdatesOutgoingRequestThreadWork)
                {
                    var outgoingFriends = _client.GetOutgoingFriendRequests();
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
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckUpdatesOutgoingRequestThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckUpdatesIncomingRequestThread()
        {
            var list = _client.GetIncomingFriendRequests();
            try
            {
                while (_isCheckUpdatesIncomingRequestThreadWork)
                {
                    var incomingFriends = _client.GetIncomingFriendRequests();

                    var list1 = list;
                    if (incomingFriends.Any(user => !list1.Contains(user))) onUpdateIncomingFriendRequestList?.Invoke(false);
                    else if (list1.Any(user => !incomingFriends.Contains(user))) onUpdateIncomingFriendRequestList?.Invoke(true);
                    list = incomingFriends;
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException) _isCheckUpdatesIncomingRequestThreadWork = false;
                else throw;
            }
        }
        private void CheckOnlineUsersThread()
        {
            var dict = _client.GetFriends().ToDictionary(friend => friend.UserId, friend => _client.GetOnlineUser(friend.UserId));
            try
            {
                while (_isCheckOnlineUsersThreadWork)
                {
                    foreach (var user in _client.GetFriends())
                    {
                        var onlineUser = _client.GetOnlineUser(user.UserId);
                        if (!dict.ContainsKey(user.UserId))
                        {
                            dict[user.UserId] = onlineUser;
                            onUpdateOnline?.Invoke(user);
                            continue;
                        }

                        if (dict[user.UserId] != onlineUser) onUpdateOnline?.Invoke(user);
                        dict[user.UserId] = onlineUser;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckOnlineUsersThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckingUpdatesOnChatListThread()
        {
            var list = _client.GetPrivateChats();
            try
            {
                while (_isCheckingUpdatesOnChatListThreadWork)
                {
                    var privateChats = _client.GetPrivateChats();
                    if (privateChats.Any(privateChat => !list.Contains(privateChat))) onUpdateChatList?.Invoke();
                    else if (list.Any(privateChat => !privateChats.Contains(privateChat))) onUpdateChatList?.Invoke();

                    list = privateChats;

                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckingUpdatesOnChatListThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckingUpdatesOnFriendListThread()
        {
            var list = _client.GetFriends();
            try
            {
                while (_isCheckingUpdatesOnFriendListThreadWork)
                {
                    var friends = _client.GetFriends();
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
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckingUpdatesOnFriendListThreadWork = false;
                else
                    throw;
            }
        }
        private void SettingOnlineThread()
        {
            try
            {
                while (_isSettingOnlineThreadWork)
                {
                    if (setOnline) _client.UpdateLastOnline();
                    Thread.Sleep(TimeSpan.FromSeconds(59));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isSettingOnlineThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckingPrivateMessageThread()
        {
            var dictionary = new Dictionary<long, long>();
            foreach (var chat in _client.GetPrivateChats())
            {
                var privateMessages = _client.GetPrivateMessagesFromChat(chat.ChatId, read: false, 1).ToList();
                if (privateMessages.Count == 0)
                {
                    dictionary[chat.ChatId] = -1;
                    continue;
                }

                var message = privateMessages[0];
                if (message is null) continue;
                dictionary[chat.ChatId] = message.MessageId;
            }

            try
            {
                while (_isCheckPrivateMessageThreadWork)
                {
                    foreach (var chat in _client.GetPrivateChats())
                    {
                        var privateMessages = _client.GetPrivateMessagesFromChat(chat.ChatId, read: false, 1).OrderByDescending(p => p.MessageId).ToList();
                        if (privateMessages.Count == 0) continue;
                        var message = privateMessages[0];
                        if (message is null) continue;
                        if (!dictionary.ContainsKey(chat.ChatId))
                        {
                            dictionary[chat.ChatId] = message.MessageId;
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
                if (e is UnauthorizedException)
                    _isCheckPrivateMessageThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckUpdateOnGuildListThread()
        {
            var list = _client.GetGuilds();
            try
            {
                while (_isCheckUpdatesOnGuildListThreadWork)
                {
                    var guilds = _client.GetGuilds();
                    if (list.Any(p => !guilds.Contains(p))) onUpdateGuildList?.Invoke();
                    else if (guilds.Any(p => !list.Contains(p))) onUpdateGuildList?.Invoke();
                    list = guilds;
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckUpdatesOnGuildListThreadWork = false;
                else throw;
            }
        }
    }
}