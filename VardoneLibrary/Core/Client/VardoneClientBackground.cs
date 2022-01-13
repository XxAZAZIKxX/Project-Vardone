using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VardoneEntities.Entities.Guild;

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
            var dictionary = _client.GetGuilds(true).ToDictionary(p => p.GuildId, p => p.Channels);

            try
            {
                while (_isCheckUpdateOnChannelsListThreadWork)
                {
                    var channelsDictionary = _client.GetGuilds(true).ToDictionary(p => p.GuildId, p => p.Channels);

                    foreach (var guildId in channelsDictionary.Keys.Except(dictionary.Keys))
                    {
                        _client.EventUpdateChannelListInvoke(_client.GetGuilds(true).FirstOrDefault(p => p.GuildId == guildId));
                    }



                    foreach (var (guildId, channels) in dictionary)
                    {
                        if (!channelsDictionary.TryGetValue(guildId, out var value)) continue;
                        if (value.Except(channels).Any())
                            _client.EventUpdateChannelListInvoke(_client.GetGuilds(true)
                                .FirstOrDefault(p => p.GuildId == guildId));
                    }

                    dictionary = channelsDictionary;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckUpdateOnChannelsListThreadWork = false;
                else throw;
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

                    list = privateChats;

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckUpdatesOnChatListThreadWork = false;
                else
                    throw;
            }
        }
        //Messages
        private void CheckingPrivateMessageThread()
        {
            var dictionary = _client.GetPrivateChats(true).ToDictionary(chat => chat.ChatId, chat => _client.GetPrivateMessagesFromChat(chat.ChatId, 1, 0, true).Select(p => p.MessageId).FirstOrDefault());

            try
            {
                while (_isCheckNewPrivateMessageThreadWork)
                {
                    var messages = _client.GetPrivateChats(true).ToDictionary(p => p.ChatId, p => _client.GetPrivateMessagesFromChat(p.ChatId, 1, 0, true).Select(m => m.MessageId).FirstOrDefault());

                    foreach (var (chatId, messageId) in dictionary)
                    {
                        if (!messages.TryGetValue(chatId, out var value)) continue;
                        if (messageId != value)
                            _client.EventNewPrivateMessageInvoke(_client.GetPrivateMessagesFromChat(chatId, 5, 0, true)
                                .FirstOrDefault(p => p.MessageId == value));
                    }

                    Thread.Sleep(200);
                }
            }
            catch (Exception)
            {
                if (_isExited)
                    _isCheckNewPrivateMessageThreadWork = false;
                else
                    throw;
            }
        }
        private void CheckNewChannelMessagesThread()
        {
            var dictionary = new Dictionary<long, long>();
            foreach (var channels in _client.GetGuilds(true).Select(p => p.Channels))
            {
                dictionary = channels.ToDictionary(p => p.ChannelId,
                    p => _client.GetChannelMessages(p.ChannelId, 1, 0, true).Select(message => message.MessageId).FirstOrDefault());
            }
            try
            {
                while (_isCheckNewChannelMessagesThreadWork)
                {
                    var messages = new Dictionary<long, long>();
                    foreach (var channels in _client.GetGuilds(true).Select(p => p.Channels))
                    {
                        messages = channels.ToDictionary(p => p.ChannelId, p => _client.GetChannelMessages(p.ChannelId, 1, 0, true).Select(message => message.MessageId).FirstOrDefault());
                    }

                    foreach (var (channelId, messageId) in dictionary)
                    {
                        if (!messages.TryGetValue(channelId, out var value)) continue;
                        if (messageId != value) _client.EventNewChannelMessageInvoke(_client.GetChannelMessage(value, true));
                    }
                    dictionary = messages;
                    Thread.Sleep(TimeSpan.FromSeconds(200));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckNewChannelMessagesThreadWork = false;
                else throw;
            }
        }
        private void CheckDeleteMessagesOnChannelThread()
        {
            var dictionary = new Dictionary<long, DateTime?>();
            foreach (var guild in _client.GetGuilds(true))
            {
                dictionary = guild.Channels.ToDictionary(p => p.ChannelId, p => _client.GetLastDeleteMessageTimeOnChannel(p.ChannelId));
            }
            try
            {
                while (_isCheckDeleteMessagesOnChannelWork)
                {
                    var times = new Dictionary<long, DateTime?>();
                    foreach (var guild in _client.GetGuilds(onlyId: true))
                    {
                        times = guild.Channels.ToDictionary(p => p.ChannelId,
                            p => _client.GetLastDeleteMessageTimeOnChannel(p.ChannelId));
                    }

                    foreach (var (channelId, time) in dictionary)
                    {
                        if (!times.TryGetValue(channelId, out var t)) continue;
                        if (time != t) _client.EventDeleteChannelMessageInvoke(_client.GetGuildChannel(channelId));
                    }
                    dictionary = times;
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckDeleteMessagesOnChannelWork = false;
                else throw;
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
                        if (dictionary[privateChat.ChatId] != lastDeleteTimeOnChat) _client.EventDeletePrivateChatMessageInvoke(privateChat);
                        else continue;
                        dictionary[privateChat.ChatId] = lastDeleteTimeOnChat;
                    }
                    Thread.Sleep(TimeSpan.FromSeconds(0.5));
                }
            }
            catch (Exception)
            {
                if (_isExited) _isCheckDeleteMessagesOnChatWork = false;
                else throw;
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
                            _client.EventUpdateOnlineInvoke(user);
                            continue;
                        }

                        if (dict[user.UserId] != onlineUser) _client.EventUpdateOnlineInvoke(user);
                        dict[user.UserId] = onlineUser;
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(10));
                }
            }
            catch (Exception)
            {
                if (_isExited)
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