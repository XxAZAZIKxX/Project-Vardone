using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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

        private bool _isExited;

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

        internal void StopThreads()
        {
            _isExited = true;
            _isCheckNewPrivateMessageThreadWork = _isSettingOnlineThreadWork = _isCheckUpdatesOnFriendListThreadWork =
                _isCheckUpdatesOnChatListThreadWork = _isCheckOnlineUsersThreadWork =
                    _isCheckUpdatesIncomingRequestThreadWork = _isCheckUpdatesOutgoingRequestThreadWork =
                        _isCheckUpdatesOnGuildListThreadWork = _isCheckUpdateOnChannelsListThreadWork =
                            _isCheckNewChannelMessagesThreadWork = _isCheckDeleteMessagesOnChatWork =
                                _isCheckDeleteMessagesOnChannelWork = false;
        }


        //Friends
        private void CheckUpdatesOutgoingRequestThread()
        {
            var list = _client.GetOutgoingFriendRequests(onlyId: true).Select(p => p.UserId).ToArray();
            try
            {
                while (_isCheckUpdatesOutgoingRequestThreadWork)
                {
                    var outgoingFriends = _client.GetOutgoingFriendRequests(onlyId: true).Select(p => p.UserId).ToArray();

                    if (list.Except(outgoingFriends).Any()) _client.EventUpdateOutgoingFriendRequestListInvoke();
                    if (outgoingFriends.Except(list).Any()) _client.EventUpdateOutgoingFriendRequestListInvoke();

                    list = outgoingFriends;

                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckUpdatesOutgoingRequestThreadWork = false;
                else throw;
            }
        }
        private void CheckUpdatesIncomingRequestThread()
        {
            var list = _client.GetIncomingFriendRequests(onlyId: true).Select(p => p.UserId).ToArray();
            try
            {
                while (_isCheckUpdatesIncomingRequestThreadWork)
                {
                    var incomingFriends = _client.GetIncomingFriendRequests(onlyId: true).Select(p => p.UserId).ToArray();

                    var list1 = list;
                    if (incomingFriends.Any(user => !list1.Contains(user))) _client.EventUpdateIncomingFriendRequestListInvoke(false);
                    else if (list1.Any(user => !incomingFriends.Contains(user))) _client.EventUpdateIncomingFriendRequestListInvoke(true);
                    list = incomingFriends;
                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckUpdatesIncomingRequestThreadWork = false;
                else throw;
            }
        }
        private void CheckingUpdatesOnFriendListThread()
        {
            var list = _client.GetFriends(true).Select(p => p.UserId).ToArray();
            try
            {
                while (_isCheckUpdatesOnFriendListThreadWork)
                {
                    var friends = _client.GetFriends(true).Select(p => p.UserId).ToArray();

                    if (list.Except(friends).Any()) _client.EventUpdateFriendListInvoke();
                    if (friends.Except(list).Any()) _client.EventUpdateFriendListInvoke();

                    list = friends;

                    Thread.Sleep(TimeSpan.FromSeconds(2));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckUpdatesOnFriendListThreadWork = false;
                else
                    throw;
            }
        }
        //Lists
        private void CheckUpdateOnGuildListThread()
        {
            var list = _client.GetGuilds(onlyId: true).Select(p => p.GuildId).ToArray();
            try
            {
                while (_isCheckUpdatesOnGuildListThreadWork)
                {
                    var guilds = _client.GetGuilds(onlyId: true).Select(p => p.GuildId).ToArray();

                    if (list.Except(guilds).Any()) _client.EventUpdateGuildListInvoke();
                    if (guilds.Except(list).Any()) _client.EventUpdateGuildListInvoke();

                    list = guilds;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckUpdatesOnGuildListThreadWork = false;
                else throw;
            }
        }
        private void CheckUpdatesOnChannelsListThread()
        {
            continueWork:
            var dictionary = _client.GetGuilds(true).ToDictionary(p => p.GuildId, p => p.Channels);

            try
            {
                while (_isCheckUpdateOnChannelsListThreadWork)
                {
                    var channelsDictionary = _client.GetGuilds(true).ToDictionary(p => p.GuildId, p => p.Channels);

                    foreach (var (guildId, channels) in dictionary)
                    {
                        if (!channelsDictionary.TryGetValue(guildId, out var value)) continue;
                        if (value.Except(channels).Any()) _client.EventUpdateChannelListInvoke(_client.GetGuild(guildId, true));
                        if (channels.Except(value).Any()) _client.EventUpdateChannelListInvoke(_client.GetGuild(guildId, true));
                    }

                    dictionary = channelsDictionary;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckUpdateOnChannelsListThreadWork = false;
                else goto continueWork;
            }
        }
        private void CheckingUpdatesOnChatListThread()
        {
            var list = _client.GetPrivateChats(onlyId: true).Select(p => p.ChatId).ToArray();
            try
            {
                while (_isCheckUpdatesOnChatListThreadWork)
                {
                    var privateChats = _client.GetPrivateChats(onlyId: true).Select(p => p.ChatId).ToArray();
                    if (list.Except(privateChats).Any()) _client.EventUpdateChatListInvoke();
                    if (privateChats.Except(list).Any()) _client.EventUpdateChatListInvoke();

                    list = privateChats;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckUpdatesOnChatListThreadWork = false;
                else throw;
            }
        }
        //Messages
        private void CheckingPrivateMessageThread()
        {
            continueWork:
            var dictionary = _client.GetPrivateChats(true).ToDictionary(chat => chat.ChatId,
                chat => _client.GetPrivateMessagesFromChat(chat.ChatId, 0, 0, true)
                    .Select(p => p.MessageId).ToArray());
            try
            {
                while (_isCheckNewPrivateMessageThreadWork)
                {
                    var messages = _client.GetPrivateChats(true).ToDictionary(chat => chat.ChatId,
                        chat => _client.GetPrivateMessagesFromChat(chat.ChatId, 0, 0, true)
                            .Select(m => m.MessageId).ToArray());

                    foreach (var (chatId, messageIds) in dictionary)
                    {
                        if (!messages.TryGetValue(chatId, out var value)) continue;
                        foreach (var messageId in value.Except(messageIds))
                        {
                            _client.EventNewPrivateMessageInvoke(_client.GetPrivateChatMessage(messageId));
                        }
                    }

                    dictionary = messages;
                    Thread.Sleep(200);
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckNewPrivateMessageThreadWork = false;
                else goto continueWork;
            }
        }
        private void CheckNewChannelMessagesThread()
        {
            continueWork:
            var dictionary = new Dictionary<long, long[]>();
            foreach (var channels in _client.GetGuilds(true).Select(p => p.Channels))
            {
                foreach (var (key, value) in channels.ToDictionary(p => p.ChannelId,
                             p => _client.GetChannelMessages(p.ChannelId, 0, 0, true)
                                 .Select(message => message.MessageId).ToArray()))
                {
                    dictionary.Add(key, value);
                }
            }
            try
            {
                while (_isCheckNewChannelMessagesThreadWork)
                {
                    var messages = new Dictionary<long, long[]>();
                    foreach (var channels in _client.GetGuilds(true).Select(p => p.Channels))
                    {
                        foreach (var (key, value) in channels.ToDictionary(p => p.ChannelId,
                                     p => _client.GetChannelMessages(p.ChannelId, 0, 0, true)
                                         .Select(message => message.MessageId).ToArray()))
                        {
                            messages.Add(key, value);
                        }
                    }

                    foreach (var (channelId, messageIds) in dictionary)
                    {
                        if (!messages.TryGetValue(channelId, out var value)) continue;
                        foreach (var messageId in value.Except(messageIds))
                        {
                            _client.EventNewChannelMessageInvoke(_client.GetChannelMessage(messageId, true));
                        }
                    }
                    dictionary = messages;
                    Thread.Sleep(200);
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckNewChannelMessagesThreadWork = false;
                else goto continueWork;
            }
        }
        private void CheckDeleteMessagesOnChannelThread()
        {
            continueWork:
            var dictionary = new Dictionary<long, long[]>();
            foreach (var guild in _client.GetGuilds(true))
                foreach (var guildChannel in guild.Channels)
                    dictionary[guildChannel.ChannelId] = _client.GetChannelMessages(guildChannel.ChannelId, 0, 0, true)
                        .Select(p => p.MessageId).ToArray();
            try
            {
                while (_isCheckDeleteMessagesOnChannelWork)
                {
                    var messages = new Dictionary<long, long[]>();
                    foreach (var guild in _client.GetGuilds(true))
                        foreach (var guildChannel in guild.Channels)
                            messages[guildChannel.ChannelId] = _client.GetChannelMessages(guildChannel.ChannelId, 0, 0, true)
                                .Select(p => p.MessageId).ToArray();

                    foreach (var (guildId, messageIds) in dictionary)
                    {
                        if (!messages.TryGetValue(guildId, out var m)) continue;
                        foreach (var deletedId in messageIds.Except(m)) _client.EventDeleteChannelMessageInvoke(deletedId);
                    }

                    dictionary = messages;
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckDeleteMessagesOnChannelWork = false;
                else goto continueWork;
            }
        }
        private void CheckDeleteMessagesOnChatThread()
        {
            continueWork:
            var dictionary = new Dictionary<long, long[]>();
            foreach (var privateChat in _client.GetPrivateChats(true))
                dictionary[privateChat.ChatId] = _client.GetPrivateMessagesFromChat(privateChat.ChatId, 0, 0, true)
                    .Select(p => p.MessageId).ToArray();
            try
            {
                while (_isCheckDeleteMessagesOnChatWork)
                {
                    var messages = new Dictionary<long, long[]>();
                    foreach (var privateChat in _client.GetPrivateChats(true))
                        messages[privateChat.ChatId] = _client.GetPrivateMessagesFromChat(privateChat.ChatId, 0, 0, true)
                            .Select(p => p.MessageId).ToArray();

                    foreach (var (chatId, messageIds) in dictionary)
                    {
                        if (!messages.TryGetValue(chatId, out var m)) continue;
                        foreach (var deletedId in messageIds.Except(m))
                            _client.EventDeletePrivateChatMessageInvoke(deletedId);
                    }

                    dictionary = messages;
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckDeleteMessagesOnChatWork = false;
                else goto continueWork;
            }
        }
        //Other
        private void CheckOnlineUsersThread()
        {
            continueWork:
            var dict = _client.GetFriends(onlyId: true).ToDictionary(friend => friend.UserId, friend => _client.GetOnlineUser(friend.UserId));
            try
            {
                while (_isCheckOnlineUsersThreadWork)
                {
                    var friends = _client.GetFriends(true).ToDictionary(p => p.UserId, p => _client.GetOnlineUser(p.UserId));

                    foreach (var (userId, online) in dict)
                    {
                        if (!friends.TryGetValue(userId, out var value)) continue;
                        if (online != value) _client.EventUpdateOnlineInvoke(_client.GetUser(userId));
                    }
                    dict = friends;
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckOnlineUsersThreadWork = false;
                else goto continueWork;
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
            catch (Exception)
            {
                if (_isExited)
                    _isSettingOnlineThreadWork = false;
                else
                    throw;
            }
        }
    }
}