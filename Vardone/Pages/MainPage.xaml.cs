using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Notification.Wpf;
using Vardone.Controls;
using Vardone.Controls.Items;
using Vardone.Core;
using Vardone.Pages.Popup;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
using VardoneLibrary.Core.Client;
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
        public static long UserId { get; private set; }

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
        }

        private void SetEventHandlers()
        {
            Client.OnDisconnect += () => Task.Run(ExitFromAccount);
            Client.OnNewPrivateMessage += OnNewPrivateMessage;
            Client.OnDeletePrivateChatMessage += OnDeletePrivateChatMessage;
            Client.OnNewChannelMessage += OnNewChannelMessage;
            Client.OnDeleteChannelMessage += OnDeleteChannelMessage;
            //
            Client.OnNewPrivateChat += OnNewPrivateChat;
            Client.OnDeletePrivateChat += OnDeletePrivateChat;
            Client.OnNewChannel += OnNewChannel;
            Client.OnUpdateChannel += OnUpdateChannel;
            Client.OnDeleteChannel += OnDeleteChannel;
            //
            Client.OnUpdateUser += OnUpdateUser;
            Client.OnUpdateUserOnline += OnUpdateUserOnline;
            //
            Client.OnNewIncomingFriendRequest += OnNewIncomingFriendRequest;
            Client.OnDeleteIncomingFriendRequest += OnDeleteIncomingFriendRequest;
            Client.OnNewOutgoingFriendRequest += OnNewOutgoingFriendRequest;
            Client.OnDeleteOutgoingFriendRequest += OnDeleteOutgoingFriendRequest;
            Client.OnNewFriend += OnNewFriend;
            Client.OnDeleteFriend += OnDeleteFriend;
            //
            Client.OnGuildJoin += OnGuildJoin;
            Client.OnGuildLeave += OnGuildLeave;
            Client.OnGuildUpdate += OnGuildUpdate;
            //
            Client.OnNewGuildInvite += OnNewGuildInvite;
            Client.OnDeleteGuildInvite += OnDeleteGuildInvite;
            //
            Client.OnNewMember += OnNewMember;
            Client.OnDeleteMember += OnDeleteMember;
            Client.OnBanMember += OnBanMember;
            Client.OnUnbanMember += OnUnbanMember;
        }

        //Events
        private Task OnNewPrivateMessage(PrivateMessage message)
        {
            return Task.Run(() =>
            {
                chatControl.AddMessage(message);
                if (message.Author.UserId == Client.GetMe().UserId) return;
                if (chatControl.Chat?.ChatId != message.Chat.ChatId)
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Новое сообщение от " + message.Author.Username,
                        Message = message.Text,
                        Type = NotificationType.None
                    });
                    Application.Current.Dispatcher.BeginInvoke(() =>
                        {
                            friendListPanel.ChatListGrid.Children.Cast<UserItem>()
                                .FirstOrDefault(p => p.User.UserId == message.Author.UserId)
                                ?.SetCountMessages(message.Chat.UnreadMessages);
                        }, DispatcherPriority.Send);
                }
            });
        }
        private Task OnDeletePrivateChatMessage(PrivateMessage message) => Task.Run(() => chatControl.DeleteMessageOnChat(message));
        private Task OnDeleteChannelMessage(ChannelMessage message) => Task.Run(() => chatControl.DeleteMessageOnChat(message));
        private Task OnNewChannelMessage(ChannelMessage message)
        {
            return Task.Run(() =>
            {
                if (chatControl.Channel?.ChannelId == message?.Channel.ChannelId) chatControl.AddMessage(message);
            });
        }
        private Task OnUnbanMember(BannedMember bannedMember) => Task.Run(() =>
        {
            Application.Current.Dispatcher.BeginInvoke(
                () => GuildMembersPage.GetInstance().RemoveBannedMember(bannedMember), DispatcherPriority.Background);
        });
        private Task OnBanMember(BannedMember bannedMember) => Task.Run(() =>
        {
            Application.Current.Dispatcher.BeginInvoke(
                () => GuildMembersPage.GetInstance().AddBannedMember(bannedMember), DispatcherPriority.Background);
        });
        private Task OnDeleteMember(Member member) => Task.Run(() =>
        {
            Application.Current.Dispatcher.BeginInvoke(() => GuildMembersPage.GetInstance().RemoveMember(member),
                DispatcherPriority.Background);
        });
        private Task OnNewMember(Member member) => Task.Run(() =>
        {
            Application.Current.Dispatcher.BeginInvoke(() => GuildMembersPage.GetInstance().AddNewMember(member), DispatcherPriority.Background);
        });
        private Task OnDeleteGuildInvite(GuildInvite invite) => Task.Run(() =>
        {
            return Application.Current.Dispatcher.BeginInvoke(
                () => GuildMembersPage.GetInstance().RemoveGuildInvite(invite),
                DispatcherPriority.Background);
        });
        private Task OnNewGuildInvite(GuildInvite invite) => Task.Run(() =>
        {
            return Application.Current.Dispatcher.BeginInvoke(
                () => GuildMembersPage.GetInstance().AddGuildInvite(invite),
                DispatcherPriority.Background);
        });

        private Task OnGuildUpdate(Guild guild)
        {
            return Task.Run(() =>
            {
                guildPanel.UpdateGuild(guild);
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    GuildList.Children.Cast<GuildListItem>().FirstOrDefault(p => p.Guild.GuildId == guild.GuildId)?.UpdateGuild(guild);
                });
            });
        }
        private Task OnGuildLeave(Guild guild)
        {
            return Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    if (guildPanel.CurrentGuild.GuildId == guild.GuildId)
                    {
                        guildPanel.ChangeGuild(null);
                        PrivateChatButtonClicked(null, null);
                    }

                    var guildListItems = GuildList.Children.Cast<GuildListItem>().ToList();
                    var firstOrDefault = guildListItems.FirstOrDefault(p => p.Guild.GuildId == guild.GuildId);
                    if (firstOrDefault is null) return;
                    GuildList.Children.RemoveAt(guildListItems.IndexOf(firstOrDefault));
                }, DispatcherPriority.Background);
            });
        }
        private Task OnGuildJoin(Guild guild) =>
            Task.Run(() => Application.Current.Dispatcher.BeginInvoke(() => GuildList.Children.Add(new GuildListItem(guild)), DispatcherPriority.Background));

        private Task OnDeleteFriend(User user)
        {
            return Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    var userItems = friendListPanel.FriendListGrid.Children.Cast<UserItem>().ToList();
                    var firstOrDefault = userItems.FirstOrDefault(p => p.User.UserId == user.UserId);
                    if (firstOrDefault is null) return;
                    friendListPanel.FriendListGrid.Children.RemoveAt(userItems.IndexOf(firstOrDefault));
                }, DispatcherPriority.Background);
            });
        }
        private Task OnNewFriend(User user)
        {
            return Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    friendListPanel.FriendListGrid.Children.Add(new UserItem(user, UserItemType.Friend));
                }, DispatcherPriority.Background);
            });
        }
        private Task OnDeleteOutgoingFriendRequest(User arg) => Task.Run(() => FriendsPropertiesPage.GetInstance().LoadOutgoingRequests());
        private Task OnNewOutgoingFriendRequest(User arg) => Task.Run(() => FriendsPropertiesPage.GetInstance().LoadOutgoingRequests());
        private Task OnDeleteIncomingFriendRequest(User _) => Task.Run(() => FriendsPropertiesPage.GetInstance().LoadIncomingRequests());
        private Task OnNewIncomingFriendRequest(User _) => Task.Run(() => FriendsPropertiesPage.GetInstance().LoadIncomingRequests());
        private Task OnUpdateUserOnline(User user)
        {
            return Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(() => friendListPanel.FriendListGrid.Children.Cast<UserItem>()
                    .FirstOrDefault(p => p.User.UserId == user.UserId)?.UpdateUserOnline(), DispatcherPriority.Background);
                Application.Current.Dispatcher.BeginInvoke(() => friendListPanel.ChatListGrid.Children.Cast<UserItem>()
                    .FirstOrDefault(p => p.User.UserId == user.UserId)?.UpdateUserOnline(), DispatcherPriority.Background);
                chatControl.UpdateUserOnline(user);
                Application.Current.Dispatcher.BeginInvoke(() => GuildMembersPage.GetInstance().UpdateMemberOnline(user), DispatcherPriority.Background);
            });
        }
        private Task OnUpdateUser(User user)
        {
            return Task.Run(() =>
            {
                AvatarsWorker.UpdateAvatarUser(user.UserId);

                Application.Current.Dispatcher.BeginInvoke(
                    () => friendListPanel.FriendListGrid.Children.Cast<UserItem>()
                        .FirstOrDefault(p => p.User.UserId == user.UserId)?.UpdateUser(user),
                    DispatcherPriority.Background);

                Application.Current.Dispatcher.BeginInvoke(
                    () => friendListPanel.ChatListGrid.Children.Cast<UserItem>()
                        .FirstOrDefault(p => p.User.UserId == user.UserId)?.UpdateUser(user),
                    DispatcherPriority.Background);
                Application.Current.Dispatcher.BeginInvoke(() => GuildMembersPage.GetInstance().UpdateMember(user), DispatcherPriority.Background);
                if (user.UserId == Client.GetMe().UserId)
                {
                    Application.Current.Dispatcher.Invoke(LoadMe);
                }
                chatControl.UpdateUser(user);
            });
        }
        private Task OnDeleteChannel(Channel channel)
        {
            return Task.Run(() =>
            {
                if (guildPanel.CurrentGuild.GuildId != channel.Guild.GuildId) return;
                guildPanel.RemoveChannel(channel);
            });
        }
        private Task OnUpdateChannel(Channel channel)
        {
            return Task.Run(() =>
            {
                if (guildPanel.CurrentGuild.GuildId != channel.Guild.GuildId) return;
                guildPanel.UpdateChannel(channel);
            });
        }
        private Task OnNewChannel(Channel channel)
        {
            return Task.Run(() =>
            {
                if (guildPanel.CurrentGuild.GuildId != channel.Guild.GuildId) return;
                guildPanel.AddChannel(channel);
            });
        }
        private Task OnDeletePrivateChat(PrivateChat chat)
        {
            return Task.Run(() =>
            {
                if (chatControl.Chat?.ChatId == chat.ChatId) chatControl.CloseChat();
                Application.Current.Dispatcher.BeginInvoke(() =>
                {
                    var userItems = friendListPanel.ChatListGrid.Children.Cast<UserItem>().ToList();
                    var item = userItems.FirstOrDefault(p => (p.User.UserId == chat.ToUser.UserId && chat.ToUser.UserId != UserId) ||
                                                             (p.User.UserId == chat.FromUser.UserId && chat.FromUser.UserId != UserId));
                    if (item is null) return;
                    var indexOf = userItems.IndexOf(item);
                    if (indexOf >= 0) friendListPanel.ChatListGrid.Children.RemoveAt(indexOf);
                }, DispatcherPriority.Send);
            });
        }
        private Task OnNewPrivateChat(PrivateChat chat)
        {
            return Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var uiElement = new UserItem(chat.ToUser, UserItemType.Chat);
                        uiElement.SetCountMessages(chat.UnreadMessages);
                        friendListPanel.ChatListGrid.Children.Add(uiElement);
                    }, DispatcherPriority.Background);
            });
        }

        public void ExitFromAccount()
        {
            Client = null;
            ConfigWorker.SetToken(null);
            AvatarsWorker.ClearAll();
            //
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                AuthorizationPage.GetInstance().OpenAuth();
                MainWindow.GetInstance().MainFrame.Navigate(AuthorizationPage.GetInstance());
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
                GuildMembersPage.ClearInstance();
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
            }, DispatcherPriority.Send);
        }

        //Loads
        public MainPage Load(VardoneClient vardoneClient)
        {
            Client = vardoneClient;
            UserId = vardoneClient.GetMe().UserId;
            LoadMe();
            LoadFriendList();
            LoadChatList();
            LoadGuilds();
            SetEventHandlers();
            return this;
        }

        private void LoadGuilds()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => GuildList.Children.Clear());

                var guilds = Client.GetGuilds().Select(p => new Guild { GuildId = p.GuildId, Name = p.Name, Owner = new Member { User = new User { UserId = p.Owner.User.UserId } } }).ToArray();
                if (guildPanel.CurrentGuild is not null && !guilds.Select(p => p.GuildId).Contains(guildPanel.CurrentGuild.GuildId))
                {
                    PrivateChatButtonClicked(null, null);
                }
                foreach (var guild in guilds)
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        GuildList.Children.Add(new GuildListItem(guild));
                    }, DispatcherPriority.Background);
                }
            });
        }
        private void LoadMe()
        {
            Task.Run(() =>
            {
                var me = Client.GetMe();
                AvatarsWorker.UpdateAvatarUser(me.UserId);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MyUsername.Text = me.Username;
                    MyAvatar.ImageSource = AvatarsWorker.GetAvatarUser(me.UserId);
                }, DispatcherPriority.Render);
            });
        }
        private void LoadChatList()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => friendListPanel.ChatListGrid.Children.Clear());
                var privateChats = Client.GetPrivateChats();
                foreach (var chat in privateChats.OrderBy(p => p.ToUser.Username).ThenByDescending(p => p.UnreadMessages))
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var friendGridItem = new UserItem(chat.ToUser, UserItemType.Chat);
                        friendGridItem.SetStatus(Client.GetOnlineUser(friendGridItem.User.UserId));
                        friendGridItem.SetCountMessages(chat.UnreadMessages);
                        friendListPanel.ChatListGrid.Children.Add(friendGridItem);
                    }, DispatcherPriority.Background);
                }
            });
        }
        private void LoadFriendList()
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() => friendListPanel.FriendListGrid.Children.Clear());

                foreach (var friend in Client.GetFriends().OrderBy(p => p.Username))
                {
                    Application.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var friendGridItem = new UserItem(friend, UserItemType.Friend);
                        friendGridItem.SetStatus(Client.GetOnlineUser(friendGridItem.User.UserId));
                        friendListPanel.FriendListGrid.Children.Add(friendGridItem);
                    }, DispatcherPriority.Background);
                }
            });
        }

        //Opens
        private void MyProfileOpen(object s, MouseEventArgs e) => UserProfileOpen(Client.GetMe(), true);
        private void AddGuildButtonMouseEnter(object sender, MouseEventArgs e) => AddButtonHover.Visibility = Visibility.Visible;
        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e) => PropertiesProfileOpen();
        private void PropertiesProfileOpen() => MainFrame.Navigate(UserPropertiesPage.GetInstance().Load());
        private void AddGuildButtonMouseLeave(object sender, MouseEventArgs e) => AddButtonHover.Visibility = Visibility.Hidden;
        private void AddGuildButtonClick(object sender, MouseButtonEventArgs e) => MainFrame.Navigate(AddGuildPage.GetInstance());
        public void UserProfileOpen(User user, bool online, bool isMe = false) => MainFrame.Navigate(UserProfilePage.GetInstance().Load(user, online, isMe));
        public void DeployImage(BitmapImage image)
        {
            DeployImagePage.GetInstance().LoadImage(image);
            MainFrame.Navigate(DeployImagePage.GetInstance());
        }
        public void OpenGuild(Guild guild)
        {
            friendListPanel.Visibility = Visibility.Collapsed;
            guildPanel.Visibility = Visibility.Visible;
            guildPanel.ChangeGuild(guild);
            chatControl.CloseChat();
            foreach (var guildItem in GuildList.Children.Cast<GuildListItem>())
                if (guildItem.Guild.GuildId == guild.GuildId)
                    guildItem.IsActive = true;
        }
        public void PrivateChatButtonClicked(object sender, MouseButtonEventArgs e)
        {
            GuildListItem.ClearAllHovers();
            friendListPanel.Visibility = Visibility.Visible;
            guildPanel.Visibility = Visibility.Collapsed;
            chatControl.CloseChat();
        }
    }
}