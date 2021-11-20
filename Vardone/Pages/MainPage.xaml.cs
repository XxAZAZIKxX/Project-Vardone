using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore.Internal;
using Notifications.Wpf;
using Vardone.Controls;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using Vardone.Pages.Popup;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneLibrary.Core.Client;
using VardoneLibrary.VardoneEvents;
using Application = System.Windows.Application;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        private static MainPage _instance;
        public static MainPage GetInstance() => _instance ??= new MainPage();
        public static VardoneClient Client { get; private set; }

        public readonly FriendPanelControl friendListPanel;
        public readonly GuildPanelControl guildPanel;
        public readonly ChatControl chatControl;

        private MainPage()
        {
            InitializeComponent();

            friendListPanel = FriendPanelControl.GetInstance();
            guildPanel = GuildPanelControl.GetInstance();
            chatControl = ChatControl.GetInstance();

            ChatListGrid.Children.Add(friendListPanel);
            ChatListGrid.Children.Add(guildPanel);
            ChatGrid.Children.Add(chatControl);
            PrivateChatButtonClicked(null, null);

            VardoneEvents.onUpdateUser += OnUpdateUser;
            VardoneEvents.onNewPrivateMessage += OnNewPrivateMessage;
            VardoneEvents.onUpdateChatList += OnUpdateChatList;
            VardoneEvents.onUpdateOnline += OnUpdateOnline;
            VardoneEvents.onUpdateIncomingFriendRequestList += OnUpdateIncomingFriendRequestList;
            VardoneEvents.onUpdateOutgoingFriendRequestList += OnUpdateOutgoingFriendRequestList;
            VardoneEvents.onUpdateFriendList += OnUpdateFriendList;
            VardoneEvents.onUpdateGuildList += LoadGuilds;
            VardoneEvents.onUpdateChannelList += OnUpdateChannelList;
            VardoneEvents.onNewChannelMessage += OnNewChannelMessage;
            VardoneEvents.onDeleteChannelMessage += OnDeleteChannelMessage;
            VardoneEvents.onDeletePrivateChatMessage += OnDeletePrivateChatMessage;
        }


        public void ExitFromAccount()
        {
            Client = null;
            JsonTokenWorker.SetToken(null);
            AvatarsWorker.ClearAll();
            //
            ChatListGrid.Children.Clear();
            ChatGrid.Children.Clear();
            //
            guildPanel.ChangeGuild(null);
            chatControl.CloseChat();
            //
            MyAvatar.ImageSource = null;
            MyUsername.Text = null;
            //
            _instance = null;
            UserPropertiesPage.ClearInstance();
            GuildPropertiesPage.ClearInstance();
            FriendsPropertiesPage.ClearInstance();
            UserProfilePage.ClearInstance();
            InviteMemberPage.ClearInstance();
            DeployImagePage.ClearInstance();
            AddGuildPage.ClearInstance();
            JoinGuildControl.ClearInstance();
            GuildPanelControl.ClearInstance();
            FriendPanelControl.ClearInstance();
            ChatControl.ClearInstance();
            //
            GC.Collect();
            //
            MainWindow.FlushMemory();
            //
            AuthorizationPage.GetInstance().OpenAuth();
            MainWindow.GetInstance().MainFrame.Navigate(AuthorizationPage.GetInstance());
        }

        //Events
        private void OnUpdateFriendList() => LoadFriendList();
        private void OnUpdateOutgoingFriendRequestList() => FriendsPropertiesPage.GetInstance().LoadOutgoingRequests();
        private void OnUpdateIncomingFriendRequestList(bool becameLess)
        {
            Task.Run(() =>
            {
                if (!becameLess)
                {
                    MainWindow.GetInstance()
                        .notificationManager.Show(new NotificationContent
                        {
                            Title = "Новые запросы в друзья",
                            Message = "Проверьте новые запросы в друзья",
                            Type = NotificationType.Information
                        });
                }

                FriendsPropertiesPage.GetInstance().LoadIncomingRequests();
            });
        }
        private void OnUpdateOnline(User user)
        {
            if (Equals(user, Client.GetMe())) return;
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var onlineUser = Client.GetOnlineUser(user.UserId);
                    foreach (var userItem in friendListPanel.FriendListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != user.UserId) continue;
                        userItem.SetStatus(onlineUser);
                        break;
                    }

                    foreach (var userItem in friendListPanel.ChatListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != user.UserId) continue;
                        userItem.SetStatus(onlineUser);
                        break;
                    }

                    foreach (var friendRequestItem in FriendsPropertiesPage.GetInstance()
                        .IncomingRequest.Children.Cast<FriendRequestItem>())
                    {
                        if (friendRequestItem.User.UserId != user.UserId) continue;
                        friendRequestItem.SetStatus(onlineUser);
                    }

                    foreach (var friendRequestItem in FriendsPropertiesPage.GetInstance()
                        .OutgoingRequest.Children.Cast<FriendRequestItem>())
                    {
                        if (friendRequestItem.User.UserId != user.UserId) continue;
                        friendRequestItem.SetStatus(onlineUser);
                    }

                    if (chatControl.chat is not null || chatControl.channel is not null)
                    {
                        if (chatControl.chat is not null) chatControl.PrivateChatHeader.Children.Cast<UserItem>().First().SetStatus(onlineUser);
                        foreach (var privateMessage in chatControl.ChatMessagesList.Children.Cast<ChatMessageItem>())
                        {
                            if (privateMessage.Author.UserId == user.UserId) privateMessage.SetStatus(onlineUser);
                        }
                    }
                });
            });
        }
        private void OnUpdateChatList() => LoadChatList();
        private void OnNewPrivateMessage(PrivateMessage message)
        {
            Task.Run(() =>
            {
                if (Equals(message.Author, Client.GetMe())) return;
                MainWindow.GetInstance()
                    .notificationManager.Show(new NotificationContent
                    {
                        Title = "Новое сообщение от " + message.Author.Username,
                        Message = message.Text,
                        Type = NotificationType.Information
                    });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (chatControl.Visibility is Visibility.Hidden or Visibility.Collapsed) return;
                    var userId = chatControl.chat?.ToUser.UserId;
                    if (userId == message.Author.UserId)
                        chatControl.LoadChat(Client.GetPrivateChatWithUser(userId.Value));
                    foreach (var userItem in friendListPanel.ChatListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != message.Author.UserId) continue;
                        userItem.SetCountMessages(Client.GetPrivateChatWithUser(message.Author.UserId).UnreadMessages);
                        break;
                    }
                });
            });
        }
        private void OnUpdateUser(User user)
        {
            Task.Run(() =>
            {
                AvatarsWorker.UpdateAvatarUser(user.UserId);
                if (Equals(user, Client.GetMe())) LoadMe();
            });
        }
        private void OnDeletePrivateChatMessage(PrivateChat chat)
        {
            if (chatControl.chat is null || chatControl.chat.ChatId != chat.ChatId) return;
            chatControl.LoadChat(chat);
        }
        private void OnDeleteChannelMessage(Channel channel)
        {
            if (chatControl.channel is null || chatControl.channel.ChannelId != channel.ChannelId) return;
            chatControl.LoadChat(channel);
        }
        private void OnNewChannelMessage(ChannelMessage message)
        {
            if (chatControl.channel is null || message is null) return;
            if (chatControl.channel.ChannelId == message.Channel.ChannelId) chatControl.LoadChat(message.Channel);
        }
        private void OnUpdateChannelList(Guild guild)
        {
            if (guildPanel.currentGuild?.GuildId == guild.GuildId) guildPanel.UpdateChannelsList();
        }

        //Loads
        public MainPage Load(VardoneClient vardoneClient)
        {
            Client = vardoneClient;
            LoadFriendList();
            LoadChatList();
            LoadGuilds();
            LoadMe();
            LoadAvatars();
            return this;
        }
        public void LoadGuilds()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildList.Children.Clear();
                foreach (var guild in Client.GetGuilds()) GuildList.Children.Add(new GuildItem(guild));
                if (guildPanel.Visibility == Visibility.Visible)
                {
                    foreach (var guildItem in GuildList.Children.Cast<GuildItem>())
                    {
                        if (guildItem.guild.GuildId == guildPanel.currentGuild.GuildId) guildItem.IsActive = true;
                    }
                }
            });
        }
        public void LoadMe()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var me = Client.GetMe();
                    MyUsername.Text = me.Username;
                    MyAvatar.ImageSource = AvatarsWorker.GetAvatarUser(me.UserId);
                });
            });
        }
        public void LoadChatList()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    friendListPanel.ChatListGrid.Children.Clear();
                    foreach (var chat in Client.GetPrivateChats()
                        .OrderBy(p => p.ToUser.Username)
                        .ThenByDescending(p => p.UnreadMessages))
                    {
                        var friendGridItem = new UserItem(chat.ToUser, UserItemType.Chat);
                        friendGridItem.SetStatus(Client.GetOnlineUser(friendGridItem.User.UserId));
                        friendGridItem.SetCountMessages(chat.UnreadMessages);
                        friendListPanel.ChatListGrid.Children.Add(friendGridItem);
                    }
                });
            });
        }
        public void LoadFriendList()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    friendListPanel.FriendListGrid.Children.Clear();
                    foreach (var friendGridItem in Client.GetFriends()
                        .OrderBy(p => p.Username)
                        .Select(friend => new UserItem(friend, UserItemType.Friend)))
                    {
                        friendGridItem.SetStatus(Client.GetOnlineUser(friendGridItem.User.UserId));
                        friendListPanel.FriendListGrid.Children.Add(friendGridItem);
                    }
                });
            });
        }
        public async void LoadAvatars()
        {
            await new Task(() =>
            {
                //Users
                foreach (var user in Client.GetFriends()) AvatarsWorker.UpdateAvatarUser(user.UserId);
                foreach (var user in Client.GetIncomingFriendRequests()) AvatarsWorker.UpdateAvatarUser(user.UserId);
                foreach (var user in Client.GetOutgoingFriendRequests()) AvatarsWorker.UpdateAvatarUser(user.UserId);
                foreach (var guildMember in Client.GetGuilds()
                    .SelectMany(guild => Client.GetGuildMembers(guild.GuildId)))
                    AvatarsWorker.UpdateAvatarUser(guildMember.UserId);
                //Guilds
                foreach (var guild in Client.GetGuilds()) AvatarsWorker.UpdateGuildAvatar(guild.GuildId);
            });
        }

        //Opens
        private void MyProfileOpen(object s, MouseEventArgs e) => UserProfileOpen(Client.GetMe(), true);
        public void UserProfileOpen(User user, bool online, bool isMe = false) => MainFrame.Navigate(UserProfilePage.GetInstance().Load(user, online, isMe));

        public void DeployImage(BitmapImage image)
        {
            DeployImagePage.GetInstance().LoadImage(image);
            MainFrame.Navigate(DeployImagePage.GetInstance());
        }
        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e) => PropertiesProfileOpen();
        private void PropertiesProfileOpen() => MainFrame.Navigate(UserPropertiesPage.GetInstance().Load());

        public void OpenGuild(Guild guild)
        {
            friendListPanel.Visibility = Visibility.Collapsed;
            guildPanel.Visibility = Visibility.Visible;
            guildPanel.ChangeGuild(guild);
            chatControl.CloseChat();
            chatControl.LoadChat(guild.Channels?.FirstOr(null!));
            foreach (var guildItem in GuildList.Children.Cast<GuildItem>())
                if (guildItem.guild.GuildId == guild.GuildId)
                    guildItem.IsActive = true;
        }
        public void PrivateChatButtonClicked(object sender, MouseButtonEventArgs e)
        {
            GuildItem.ClearAllHovers();
            friendListPanel.Visibility = Visibility.Visible;
            guildPanel.Visibility = Visibility.Collapsed;
            chatControl.CloseChat();
        }
        private void AddGuildButtonMouseEnter(object sender, MouseEventArgs e) => AddButtonHover.Visibility = Visibility.Visible;
        private void AddGuildButtonMouseLeave(object sender, MouseEventArgs e) => AddButtonHover.Visibility = Visibility.Hidden;
        private void AddGuildButtonClick(object sender, MouseButtonEventArgs e) => MainFrame.Navigate(AddGuildPage.GetInstance());
    }
}