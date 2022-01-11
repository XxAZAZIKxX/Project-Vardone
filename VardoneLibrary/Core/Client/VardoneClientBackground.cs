using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VardoneEntities.Entities.Guild;
using VardoneLibrary.Exceptions;

namespace VardoneLibrary.Core.Client
{
    internal class VardoneClientBackground
    {
        private VardoneClient _client;

        private Thread _checkNewPrivateMessageThread;
        private bool _isCheckNewPrivateMessageThreadWork = true;
        //
        private Thread _settingOnlineThread;
        private bool _isSettingOnlineThreadWork = true;
        //
        private Thread _checkUpdatesOnFriendListThread;
        private bool _isCheckUpdatesOnFriendListThreadWork = true;
        //
        private Thread _checkUpdatesOnChatListThread;
        private bool _isCheckUpdatesOnChatListThreadWork = true;
        //
        private Thread _checkOnlineUsersThread;
        private bool _isCheckOnlineUsersThreadWork = true;
        //
        private Thread _checkUpdatesIncomingRequestThread;
        private bool _isCheckUpdatesIncomingRequestThreadWork = true;
        //
        private Thread _checkUpdatesOutgoingRequestThread;
        private bool _isCheckUpdatesOutgoingRequestThreadWork = true;
        //
        private Thread _checkUpdatesOnGuildListThread;
        private bool _isCheckUpdatesOnGuildListThreadWork = true;
        //
        private Thread _checkUpdatesOnChannelsListThread;
        private bool _isCheckUpdateOnChannelsListThreadWork = true;
        //
        private Thread _checkNewChannelMessagesThread;
        private bool _isCheckNewChannelMessagesThreadWork = true;
        //
        private Thread _checkDeleteMessagesOnChat;
        private bool _isCheckDeleteMessagesOnChatWork = true;
        //
        private Thread _checkDeleteMessagesOnChannel;
        private bool _isCheckDeleteMessagesOnChannelWork = true;
        //
        internal bool setOnline = true;
        //

        public VardoneClientBackground(VardoneClient client)
        {
            _client = client;
            SetThreads();
        }
        ~VardoneClientBackground()
        {
            StopThreads();
            _client = null;
            GC.Collect();
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
            //
            _checkDeleteMessagesOnChat = new Thread(CheckDeleteMessagesOnChatThread);
            _checkDeleteMessagesOnChat.Start();
            //
            _checkDeleteMessagesOnChannel = new Thread(CheckDeleteMessagesOnChannelThread);
            _checkDeleteMessagesOnChannel.Start();
        }

        internal void StopThreads() =>
            _isCheckNewPrivateMessageThreadWork = _isSettingOnlineThreadWork = _isCheckUpdatesOnFriendListThreadWork =
                _isCheckUpdatesOnChatListThreadWork = _isCheckOnlineUsersThreadWork =
                    _isCheckUpdatesIncomingRequestThreadWork = _isCheckUpdatesOutgoingRequestThreadWork =
                        _isCheckUpdatesOnGuildListThreadWork = _isCheckUpdateOnChannelsListThreadWork =
                            _isCheckNewChannelMessagesThreadWork = _isCheckDeleteMessagesOnChatWork =
                                _isCheckDeleteMessagesOnChannelWork = false;


        //Friends
        private void CheckUpdatesOutgoingRequestThread()
        {
            var list = _client.GetOutgoingFriendRequests(onlyId: true);
            try
            {
                while (_isCheckUpdatesOutgoingRequestThreadWork)
                {
                    var outgoingFriends = _client.GetOutgoingFriendRequests(onlyId: true);

                    if (outgoingFriends.Any(user => !list.Contains(user))) _client.UpdateOutgoingFriendRequestList();
                    else if (list.Any(user => !outgoingFriends.Contains(user))) _client.UpdateOutgoingFriendRequestList();
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
            var list = _client.GetIncomingFriendRequests(onlyId: true);
            try
            {
                while (_isCheckUpdatesIncomingRequestThreadWork)
                {
                    var incomingFriends = _client.GetIncomingFriendRequests(onlyId: true);

                    var list1 = list;
                    if (incomingFriends.Any(user => !list1.Contains(user))) _client.UpdateIncomingFriendRequestList(false);
                    else if (list1.Any(user => !incomingFriends.Contains(user))) _client.UpdateIncomingFriendRequestList(true);
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
            var list = _client.GetFriends(true);
            try
            {
                while (_isCheckUpdatesOnFriendListThreadWork)
                {
                    var friends = _client.GetFriends();

                    if (friends.Any(user => !list.Contains(user))) _client.UpdateFriendList();
                    else if (list.Any(user => !friends.Contains(user))) _client.UpdateFriendList();
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
            var list = _client.GetGuilds(onlyId: true);
            try
            {
                while (_isCheckUpdatesOnGuildListThreadWork)
                {
                    var guilds = _client.GetGuilds(onlyId: true);
                    if (list.Any(p => !guilds.Contains(p))) _client.UpdateGuildList();
                    else if (guilds.Any(p => !list.Contains(p))) _client.UpdateGuildList();
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
            foreach (var guild in _client.GetGuilds()) list.AddRange(_client.GetGuildChannels(guild.GuildId, onlyId: true));
            try
            {
                while (_isCheckUpdateOnChannelsListThreadWork)
                {
                    var channels = new List<Channel>();
                    foreach (var guild in _client.GetGuilds()) channels.AddRange(_client.GetGuildChannels(guild.GuildId, onlyId: true));

                    foreach (var p in channels.Where(p => !list.Contains(p))) _client.UpdateChannelList(p.Guild);
                    foreach (var p in list.Where(p => !channels.Contains(p))) _client.UpdateChannelList(p.Guild);

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
            var list = _client.GetPrivateChats(onlyId: true);
            try
            {
                while (_isCheckUpdatesOnChatListThreadWork)
                {
                    var privateChats = _client.GetPrivateChats(onlyId: true);
                    if (privateChats.Any(privateChat => !list.Contains(privateChat))) _client.UpdateChatList();
                    else if (list.Any(privateChat => !privateChats.Contains(privateChat))) _client.UpdateChatList();

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
            foreach (var chat in _client.GetPrivateChats(onlyId: true))
            {
                var privateMessages = _client.GetPrivateMessagesFromChat(chat.ChatId, 1, 0, onlyId: true);
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
                    foreach (var chat in _client.GetPrivateChats(onlyId: true))
                    {
                        var privateMessages = _client.GetPrivateMessagesFromChat(chat.ChatId, 1, 0, onlyId: true);
                        if (privateMessages.Count == 0) continue;
                        var message = privateMessages[0];
                        if (message is null || message.Author.UserId == _client.GetMe().UserId) continue;
                        if (!dictionary.ContainsKey(chat.ChatId))
                        {
                            dictionary[chat.ChatId] = message.MessageId;
                            _client.NewPrivateMessage(message);
                        }

                        if (dictionary[chat.ChatId] == message.MessageId) continue;
                        dictionary[chat.ChatId] = message.MessageId;
                        _client.NewPrivateMessage(message);
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
            foreach (var guild in _client.GetGuilds(onlyId: true))
            {
                if (guild.Channels is null) continue;
                foreach (var guildChannel in guild.Channels)
                {
                    var channelMessage = _client.GetChannelMessages(guildChannel.ChannelId, 1, 0, onlyId: true).FirstOrDefault();
                    dictionary[guildChannel.ChannelId] = channelMessage?.MessageId;
                }
            }
            try
            {
                while (_isCheckNewChannelMessagesThreadWork)
                {
                    foreach (var guild in _client.GetGuilds(onlyId: true))
                    {
                        if (guild.Channels is null) continue;
                        foreach (var guildChannel in guild.Channels)
                        {
                            var channelMessage = _client.GetChannelMessages(guildChannel.ChannelId, 1, 0, onlyId: true).FirstOrDefault();
                            if (!dictionary.ContainsKey(guildChannel.ChannelId) || dictionary[guildChannel.ChannelId] != channelMessage?.MessageId)
                            {
                                _client.NewChannelMessage(channelMessage);
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
        private void CheckDeleteMessagesOnChannelThread()
        {
            var dictionary = new Dictionary<long, DateTime?>();
            foreach (Guild guild in _client.GetGuilds(onlyId: true))
            {
                foreach (Channel guildChannel in _client.GetGuildChannels(guild.GuildId))
                {
                    dictionary[guildChannel.ChannelId] = _client.GetLastDeleteMessageTimeOnChannel(guildChannel.ChannelId);
                }
            }

            try
            {
                while (_isCheckDeleteMessagesOnChannelWork)
                {
                    foreach (var guild in _client.GetGuilds(onlyId: true))
                    {
                        foreach (var guildChannel in guild.Channels)
                        {
                            var lastDeleteMessageTimeOnChannel = _client.GetLastDeleteMessageTimeOnChannel(guildChannel.ChannelId);
                            if (dictionary[guildChannel.ChannelId] == lastDeleteMessageTimeOnChannel) continue;
                            _client.DeleteChannelMessage(guildChannel);
                            dictionary[guildChannel.ChannelId] = lastDeleteMessageTimeOnChannel;
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException) _isCheckDeleteMessagesOnChannelWork = false;
            }
        }
        private void CheckDeleteMessagesOnChatThread()
        {
            var dictionary = new Dictionary<long, DateTime?>();
            foreach (var privateChat in _client.GetPrivateChats(onlyId: true))
            {
                dictionary[privateChat.ChatId] = _client.GetLastDeleteTimeOnChat(privateChat.ChatId);
            }
            try
            {
                while (_isCheckDeleteMessagesOnChatWork)
                {
                    foreach (var privateChat in _client.GetPrivateChats(onlyId: true))
                    {
                        var lastDeleteTimeOnChat = _client.GetLastDeleteTimeOnChat(privateChat.ChatId);
                        if (dictionary[privateChat.ChatId] != lastDeleteTimeOnChat) _client.DeletePrivateChatMessage(privateChat);
                        else continue;
                        dictionary[privateChat.ChatId] = lastDeleteTimeOnChat;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception e)
            {
                if (e is UnauthorizedException) _isCheckDeleteMessagesOnChatWork = false;
            }
        }
        //Other
        private void CheckOnlineUsersThread()
        {
            var dict = _client.GetFriends(onlyId: true).ToDictionary(friend => friend.UserId, friend => _client.GetOnlineUser(friend.UserId));
            try
            {
                while (_isCheckOnlineUsersThreadWork)
                {
                    foreach (var user in _client.GetFriends(onlyId: true))
                    {
                        var onlineUser = _client.GetOnlineUser(user.UserId);
                        if (!dict.ContainsKey(user.UserId))
                        {
                            dict[user.UserId] = onlineUser;
                            _client.UpdateOnline(user);
                            continue;
                        }

                        if (dict[user.UserId] != onlineUser) _client.UpdateOnline(user);
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