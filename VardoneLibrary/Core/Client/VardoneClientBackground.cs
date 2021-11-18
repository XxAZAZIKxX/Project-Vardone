using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VardoneEntities.Entities.Guild;
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
        private Thread _checkNewPrivateMessageThread;
        private bool _isCheckNewPrivateMessageThreadWork = true;
        /// <summary>
        /// Поток который обновляет онлайн текущего пользователя
        /// </summary>
        private Thread _settingOnlineThread;
        private bool _isSettingOnlineThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновления списка друзей
        /// </summary>
        private Thread _checkUpdatesOnFriendListThread;
        private bool _isCheckUpdatesOnFriendListThreadWork = true;
        /// <summary>
        /// Поток проверяющий на обновления списка чатов
        /// </summary>
        private Thread _checkUpdatesOnChatListThread;
        private bool _isCheckUpdatesOnChatListThreadWork = true;
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
        /// Поток проверяющий на обновления списка каналов на сервере
        /// </summary>
        private Thread _checkUpdatesOnChannelsListThread;
        private bool _isCheckUpdateOnChannelsListThreadWork = true;
        /// <summary>
        /// Поток проверяющий на новые сообщения в канале на сервере
        /// </summary>
        private Thread _checkNewChannelMessagesThread;
        private bool _isCheckNewChannelMessagesThreadWork = true;
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
            _checkNewPrivateMessageThread = new Thread(CheckingPrivateMessageThread);
            _checkNewPrivateMessageThread.Start();
            //
            _settingOnlineThread = new Thread(SettingOnlineThread);
            _settingOnlineThread.Start();
            //
            _checkUpdatesOnFriendListThread = new Thread(CheckingUpdatesOnFriendListThread);
            _checkUpdatesOnFriendListThread.Start();
            //
            _checkUpdatesOnChatListThread = new Thread(CheckingUpdatesOnChatListThread);
            _checkUpdatesOnChatListThread.Start();
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
            //
            _checkUpdatesOnChannelsListThread = new Thread(CheckUpdatesOnChannelsListThread);
            _checkUpdatesOnChannelsListThread.Start();
            //
            _checkNewChannelMessagesThread = new Thread(CheckNewChannelMessagesThread);
            _checkNewChannelMessagesThread.Start();
        }
        internal void StopThreads() => _isCheckNewPrivateMessageThreadWork = _isSettingOnlineThreadWork =
            _isCheckUpdatesOnFriendListThreadWork = _isCheckUpdatesOnChatListThreadWork =
                _isCheckOnlineUsersThreadWork = _isCheckUpdatesIncomingRequestThreadWork =
                    _isCheckUpdatesOutgoingRequestThreadWork = _isCheckUpdatesOnGuildListThreadWork =
                        _isCheckUpdateOnChannelsListThreadWork = _isCheckNewChannelMessagesThreadWork = false;


        //Friends
        private void CheckUpdatesOutgoingRequestThread()
        {
            var list = _client.GetOutgoingFriendRequests();
            try
            {
                while (_isCheckUpdatesOutgoingRequestThreadWork)
                {
                    var outgoingFriends = _client.GetOutgoingFriendRequests();

                    if (outgoingFriends.Any(user => !list.Contains(user))) onUpdateOutgoingFriendRequestList?.Invoke();
                    else if (list.Any(user => !outgoingFriends.Contains(user))) onUpdateOutgoingFriendRequestList?.Invoke();
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
        private void CheckingUpdatesOnFriendListThread()
        {
            var list = _client.GetFriends();
            try
            {
                while (_isCheckUpdatesOnFriendListThreadWork)
                {
                    var friends = _client.GetFriends();

                    if (friends.Any(user => !list.Contains(user))) onUpdateFriendList?.Invoke();
                    if (list.Any(user => !friends.Contains(user))) onUpdateFriendList?.Invoke();
                    list = friends;
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckUpdatesOnFriendListThreadWork = false;
                else
                    throw;
            }
        }
        //Lists
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
        private void CheckUpdatesOnChannelsListThread()
        {
            var list = new List<Channel>();
            foreach (var guild in _client.GetGuilds()) list.AddRange(_client.GetGuildChannels(guild.GuildId));
            try
            {
                while (_isCheckUpdateOnChannelsListThreadWork)
                {
                    var channels = new List<Channel>();
                    foreach (var guild in _client.GetGuilds()) channels.AddRange(_client.GetGuildChannels(guild.GuildId));

                    foreach (var p in channels.Where(p => !list.Contains(p))) onUpdateChannelList?.Invoke(p.Guild);
                    foreach (var p in list.Where(p => !channels.Contains(p))) onUpdateChannelList?.Invoke(p.Guild);

                    list = channels;
                    Thread.Sleep(TimeSpan.FromSeconds(1.5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException)
                    _isCheckUpdateOnChannelsListThreadWork = false;
                else throw;
            }
        }
        private void CheckingUpdatesOnChatListThread()
        {
            var list = _client.GetPrivateChats();
            try
            {
                while (_isCheckUpdatesOnChatListThreadWork)
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
                    _isCheckUpdatesOnChatListThreadWork = false;
                else
                    throw;
            }
        }
        //Messages
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
                while (_isCheckNewPrivateMessageThreadWork)
                {
                    foreach (var chat in _client.GetPrivateChats())
                    {
                        var privateMessages = _client.GetPrivateMessagesFromChat(chat.ChatId, read: false, 1).OrderByDescending(p => p.MessageId).ToList();
                        if (privateMessages.Count == 0) continue;
                        var message = privateMessages[0];
                        if (message is null || message.Author.UserId == _client.GetMe().UserId) continue;
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
                    _isCheckNewPrivateMessageThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckNewChannelMessagesThread()
        {
            var dictionary = new Dictionary<long, long?>();
            foreach (var guild in _client.GetGuilds())
            {
                if (guild.Channels is null) continue;
                foreach (var guildChannel in guild.Channels)
                {
                    var channelMessage = _client.GetChannelMessages(guildChannel.ChannelId, 1).FirstOrDefault();
                    dictionary[guildChannel.ChannelId] = channelMessage?.MessageId;
                }
            }
            try
            {
                while (_isCheckNewChannelMessagesThreadWork)
                {
                    foreach (var guild in _client.GetGuilds())
                    {
                        if (guild.Channels is null) continue;
                        foreach (var guildChannel in guild.Channels)
                        {
                            var channelMessage = _client.GetChannelMessages(guildChannel.ChannelId, 1).FirstOrDefault();
                            if (!dictionary.ContainsKey(guildChannel.ChannelId) || dictionary[guildChannel.ChannelId] != channelMessage?.MessageId)
                            {
                                onNewChannelMessage?.Invoke(channelMessage);
                                dictionary[guildChannel.ChannelId] = channelMessage?.MessageId;
                            }
                        }
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException) _isCheckNewChannelMessagesThreadWork = false;
            }
        }
        //Other
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

    }
}