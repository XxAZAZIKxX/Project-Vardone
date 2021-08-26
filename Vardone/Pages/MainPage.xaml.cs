using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Notifications.Wpf;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Client;
using VardoneLibrary.VardoneEvents;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
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


        private MainPage()
        {
            InitializeComponent();

            VardoneEvents.onUpdateUser += OnUpdateUser;
            VardoneEvents.onNewPrivateMessage += OnNewPrivateMessage;
            VardoneEvents.onUpdateChatList += OnUpdateChatList;
            VardoneEvents.onUpdateOnline += OnUpdateOnline;
            VardoneEvents.onUpdateIncomingFriendRequestList += OnUpdateIncomingFriendRequestList;
            VardoneEvents.onUpdateOutgoingFriendRequestList += OnUpdateOutgoingFriendRequestList;
            VardoneEvents.onUpdateFriendList += OnUpdateFriendList;
        }

        public void ExitFromAccount()
        {
            Client = null;
            ChatListGrid.Children.Clear();
            FriendListGrid.Children.Clear();
            PrivateChatHeader.Children.Clear();
            ChatMessagesGrid.Children.Clear();
            MyAvatar.ImageSource = null;
            MyUsername.Text = null;
            JsonTokenWorker.SetToken(null);
            _instance = null;
            AuthorizationPage.GetInstance().OpenAuth();
            MainWindow.GetInstance().MainFrame.Navigate(AuthorizationPage.GetInstance());
        }

        //Events
        private void OnUpdateFriendList() => LoadFriendList();

        private void OnUpdateOutgoingFriendRequestList() => FriendsProperties.GetInstance().LoadOutgoingRequests();

        private void OnUpdateIncomingFriendRequestList(bool becameLess)
        {
            Task.Run(() =>
            {
                if (!becameLess)
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Новые запросы в друзья",
                        Message = "Проверьте новые запросы в друзья",
                        Type = NotificationType.Information
                    });
                }
                FriendsProperties.GetInstance().LoadIncomingRequests();
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
                    foreach (var userItem in FriendListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != user.UserId) continue;
                        userItem.SetStatus(onlineUser);
                        break;
                    }

                    foreach (var userItem in ChatListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != user.UserId) continue;
                        userItem.SetStatus(onlineUser);
                        break;
                    }

                    foreach (var friendRequestItem in FriendsProperties.GetInstance().IncomingRequest.Children.Cast<FriendRequestItem>())
                    {
                        if (friendRequestItem.User.UserId != user.UserId) continue;
                        friendRequestItem.SetStatus(onlineUser);
                    }

                    foreach (var friendRequestItem in FriendsProperties.GetInstance().OutgoingRequest.Children.Cast<FriendRequestItem>())
                    {
                        if (friendRequestItem.User.UserId != user.UserId) continue;
                        friendRequestItem.SetStatus(onlineUser);
                    }

                    if (PrivateChatHeader.Children.Count > 0)
                    {
                        var userItem = (UserItem)PrivateChatHeader.Children[0];
                        if (userItem.User.UserId != user.UserId) return;
                        userItem.SetStatus(onlineUser);
                        foreach (var privateMessage in ChatMessagesGrid.Children.Cast<ChatMessageItem>())
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
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Новое сообщение от " + message.Author.Username,
                    Message = message.Text,
                    Type = NotificationType.Information
                });
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (PrivateChatHeader.Children.Count == 0) return;
                    var userId = ((UserItem)PrivateChatHeader.Children[0]).User.UserId;
                    if (userId == message.Author.UserId) LoadPrivateChat(userId);
                    foreach (var userItem in ChatListGrid.Children.Cast<UserItem>())
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

        private void ChatScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ChatScrollViewer.VerticalOffset != 0) return;
                    if (PrivateChatHeader.Children.Count == 0) return;
                    if (ChatMessagesGrid.Children.Count == 0) return;
                    var user = ((UserItem)PrivateChatHeader.Children[0]).User;

                    var onlineUser1 = Client.GetOnlineUser(user.UserId);
                    var onlineUser2 = Client.GetOnlineUser(Client.GetMe().UserId);
                    var privateMessagesFromChat = Client.GetPrivateMessagesFromChat(
                        Client.GetPrivateChatWithUser(user.UserId).ChatId, 5, ((ChatMessageItem)ChatMessagesGrid.Children[0]).Message.MessageId);
                    foreach (var message in privateMessagesFromChat)
                    {
                        var messageItem = new ChatMessageItem(message);
                        messageItem.SetStatus(message.Author.UserId == user.UserId ? onlineUser1 : onlineUser2);
                        ChatMessagesGrid.Children.Insert(0, messageItem);
                    }

                    if (privateMessagesFromChat.Count > 0) ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
                });
            });
        }

        //Loads

        public void Load(VardoneClient vardoneClient)
        {
            Client = vardoneClient;
            LoadFriendList();
            LoadChatList();
            LoadMe();
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
                    ChatListGrid.Children.Clear();
                    foreach (var chat in Client.GetPrivateChats().OrderBy(p => p.ToUser.Username).ThenByDescending(p => p.UnreadMessages))
                    {
                        var friendGridItem = new UserItem(chat.ToUser, UserItemType.Chat);
                        friendGridItem.SetStatus(Client.GetOnlineUser(friendGridItem.User.UserId));
                        friendGridItem.SetCountMessages(chat.UnreadMessages);
                        ChatListGrid.Children.Add(friendGridItem);
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
                    FriendListGrid.Children.Clear();
                    foreach (var friendGridItem in Client.GetFriends().OrderBy(p => p.Username).Select(friend => new UserItem(friend, UserItemType.Friend)))
                    {
                        friendGridItem.SetStatus(Client.GetOnlineUser(friendGridItem.User.UserId));
                        FriendListGrid.Children.Add(friendGridItem);
                    }
                });
            });
        }

        public void LoadPrivateChat(long userId)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatMessagesGrid.Children.Clear();
                    PrivateChatHeader.Children.Clear();
                    var user = Client.GetUser(userId);
                    var userHeader = new UserItem(user, UserItemType.Friend)
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    var onlineUser = Client.GetOnlineUser(user.UserId);
                    userHeader.SetStatus(onlineUser);
                    PrivateChatHeader.Children.Add(userHeader);

                    var chatId = Client.GetPrivateChatWithUser(userId).ChatId;

                    foreach (var message in Client.GetPrivateMessagesFromChat(chatId, 5).OrderBy(p => p.MessageId))
                    {
                        var messageItem = new ChatMessageItem(message);
                        if (messageItem.Author.UserId == user.UserId) messageItem.SetStatus(onlineUser);
                        ChatMessagesGrid.Children.Add(messageItem);
                    }
                    var privateChatWithUser = Client.GetPrivateChatWithUser(userId);
                    foreach (var userItem in ChatListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != userId) continue;
                        userItem.SetCountMessages(privateChatWithUser.UnreadMessages);
                        break;
                    }


                    ChatScrollViewer.ScrollToEnd();
                });
            });
        }

        //Opens
        private void MyProfileOpen(object s, MouseEventArgs e) => UserProfileOpen(Client.GetMe(), true);

        public void UserProfileOpen(User user, bool online, bool isMe = false)
        {
            if (isMe) online = Client.SetOnline;
            UserProfilePage.GetInstance().Load(user, online, isMe);
            MainFrame.Navigate(UserProfilePage.GetInstance());
        }

        public void DeployImage(BitmapImage image)
        {
            DeployImagePage.GetInstance().LoadImage(image);
            MainFrame.Navigate(DeployImagePage.GetInstance());
        }

        private void PropertiesButtonClick(object sender, MouseButtonEventArgs e) => GetInstance().PropertiesProfileOpen();

        public void PropertiesProfileOpen()
        {
            PropertiesPage.GetInstance().Load();
            MainFrame.Navigate(PropertiesPage.GetInstance());
        }

        private void ClipMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (PrivateChatHeader.Children.Count == 0) return;
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg",
                Multiselect = false
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            var userId = ((UserItem)PrivateChatHeader.Children[0]).User.UserId;
            Client.SendPrivateMessage(userId,
                new MessageModel
                {
                    Text = MessageTextBox.Text,
                    Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
                });
            MessageTextBox.Clear();
            LoadPrivateChat(userId);
        }

        private void OpenFriendsProperties(object sender, MouseButtonEventArgs e)
        {
            FriendsProperties.GetInstance().Load();
            MainFrame.Navigate(FriendsProperties.GetInstance());
        }

        //Placeholders

        private void MessageBoxGotFocus(object sender, RoutedEventArgs e) =>
            MessageTextBoxPlaceholder.Visibility = Visibility.Collapsed;

        private void MessageBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageTextBox.Text)) return;
            MessageTextBoxPlaceholder.Visibility = Visibility.Visible;
        }

        private void MessageTextBoxOnTextChanged(object sender, TextChangedEventArgs e) =>
            MessageBoxGotFocus(null, null);

        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text)) return;
            if (PrivateChatHeader.Children.Count == 0) return;
            if (PrivateChatHeader.Children[0] is not UserItem) return;
            var user = ((UserItem)PrivateChatHeader.Children[0]).User;
            Client.SendPrivateMessage(user.UserId, new MessageModel { Text = MessageTextBox.Text });
            MessageTextBox.Text = "";
            MessageBoxLostFocus(null, null);
            LoadPrivateChat(user.UserId);
        }
    }
}