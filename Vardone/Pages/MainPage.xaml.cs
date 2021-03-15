using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Notifications.Wpf;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneLibrary.Core;
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
        public static VardoneClient client;
        public static BitmapImage DefaultAvatar { get; private set; }
        public static Dictionary<long, BitmapImage> UserAvatars { get; set; } = new();

        private MainPage()
        {
            InitializeComponent();
            DefaultAvatar = ImageWorker.BytesToBitmapImage(File.ReadAllBytes(MainWindow.PATH + @"\resources\avatar.jpg"));

            VardoneEvents.onUpdateUser += OnUpdateUser;
            VardoneEvents.onNewPrivateMessage += OnNewPrivateMessage;
            VardoneEvents.onUpdateChatList += OnUpdateChatList;
            VardoneEvents.onUpdateOnline += OnUpdateOnline;
            VardoneEvents.onUpdateIncomingFriendRequestList += OnUpdateIncomingFriendRequestList;
            VardoneEvents.onUpdateOutgoingFriendRequestList += OnUpdateOutgoingFriendRequestList;
            VardoneEvents.onUpdateFriendList += OnUpdateFriendList;
        }

        private void OnUpdateFriendList() => LoadFriendList();

        private void OnUpdateOutgoingFriendRequestList()
        {
        }

        private void OnUpdateIncomingFriendRequestList()
        {
            MainWindow.GetInstance().notificationManager.Show(new NotificationContent
            {
                Title = "Новые запросы в друзья",
                Message = "Проверьте новый запрос в друзья",
                Type = NotificationType.Information
            });
        }

        private void OnUpdateOnline(User user)
        {
        }

        private void OnUpdateChatList() => LoadChatList();

        private void OnNewPrivateMessage(PrivateMessage message)
        {
            if (message.Author.UserId == client.UserId) return;
            MainWindow.GetInstance().notificationManager.Show(new NotificationContent
            {
                Title = "Новое сообщение от " + message.Author.Username,
                Message = message.Text,
                Type = NotificationType.Information
            });
            Dispatcher.Invoke(() =>
            {
                if (ChatHeader.Children.Count == 0) return;
                var userId = ((UserItem)ChatHeader.Children[0]).user.UserId;
                if (userId == message.Author.UserId) LoadPrivateChat(userId);
            });
        }

        private void OnUpdateUser(User user)
        {
            UserAvatars[user.UserId] = user.Base64Avatar switch
            {
                null => DefaultAvatar,
                _ => ImageWorker.BytesToBitmapImage(Convert.FromBase64String(user.Base64Avatar))
            };
            if (user.UserId == client.UserId) LoadMe();
        }

        public void Load(VardoneClient vardoneClient)
        {
            client = vardoneClient;
            LoadFriendList();
            LoadChatList();
            LoadMe();
        }

        public void LoadMe()
        {
            var me = client.GetMe();
            if (!UserAvatars.ContainsKey(me.UserId))
                UserAvatars.Add(me.UserId, me.Base64Avatar is null ? DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(me.Base64Avatar)));

            MyUsername.Text = me.Username;
            MyAvatar.ImageSource = UserAvatars[me.UserId];
        }

        public void LoadChatList()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                ChatListGrid.Children.Clear();
                foreach (var friendGridItem in client.GetPrivateChats()
                    .Select(chat => new UserItem(chat.ToUser, MouseDownEventLogic.OpenChat)))
                {
                    ChatListGrid.Children.Add(friendGridItem);
                }
            });
        }

        public void LoadFriendList()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                FriendListGrid.Children.Clear();
                foreach (var friendGridItem in client.GetFriends().OrderBy(p => p.Username)
                    .Select(friend => new UserItem(friend, MouseDownEventLogic.OpenProfile)))
                {
                    FriendListGrid.Children.Add(friendGridItem);
                }
            });
        }

        public void LoadPrivateChat(long userId)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                ChatMessagesGrid.Children.Clear();
                ChatHeader.Children.Clear();
                var user1 = client.GetUser(userId);
                var userItem = new UserItem(user1, MouseDownEventLogic.OpenProfile)
                {
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Left
                };
                ChatHeader.Children.Add(userItem);
                foreach (var message in client
                    .GetPrivateMessagesFromChat(client.GetPrivateChatWithUser(userId).ChatId, 5)
                    .OrderBy(p => p.MessageId))
                {
                    var messageItem = new MessageChatItem(message);
                    ChatMessagesGrid.Children.Add(messageItem);
                }

                ChatScrollViewer.ScrollToEnd();
            });
            ChatScrollViewer.ScrollChanged += (_, _) =>
            {
                if (ChatScrollViewer.VerticalOffset != 0) return;
                if (ChatHeader.Children.Count == 0) return;
                if (ChatMessagesGrid.Children.Count == 0) return;
                var privateMessagesFromChat = client.GetPrivateMessagesFromChat(client.GetPrivateChatWithUser(((UserItem)ChatHeader.Children[0]).user.UserId).ChatId, 5,
                    ((MessageChatItem)ChatMessagesGrid.Children[0]).message.MessageId);
                foreach (var messageItem in privateMessagesFromChat.Select(message => new MessageChatItem(message)))
                {
                    ChatMessagesGrid.Children.Insert(0, messageItem);
                }

                if (privateMessagesFromChat.Count > 0)
                    ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
            };
        }

        private void MyProfileOpen(object s, MouseEventArgs e) => UserProfileOpen(client.GetMe(), true);

        public void UserProfileOpen(User user, bool isMe = false)
        {
            UserProfilePage.GetInstance().Load(user, isMe);
            MainFrame.Navigate(UserProfilePage.GetInstance());
        }

        public void DeployImage(BitmapImage image)
        {
            DeployImagePage.GetInstance().LoadImage(image);
            MainFrame.Navigate(DeployImagePage.GetInstance());
        }

        private void MessageBoxGotFocus(object sender, RoutedEventArgs e) => MessageTextBoxPlaceholder.Visibility = Visibility.Collapsed;

        private void MessageBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageTextBox.Text)) return;
            MessageTextBoxPlaceholder.Visibility = Visibility.Visible;
        }

        private void MessageTextBoxOnTextChanged(object sender, TextChangedEventArgs e) => MessageBoxGotFocus(null, null);

        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text)) return;
            if (ChatHeader.Children.Count == 0) return;
            if (ChatHeader.Children[0] is not UserItem) return;
            var user = ((UserItem)ChatHeader.Children[0]).user;
            client.SendPrivateMessage(user.UserId, new PrivateMessageModel { Text = MessageTextBox.Text });
            MessageTextBox.Text = "";
            MessageBoxLostFocus(null, null);
            LoadPrivateChat(user.UserId);
        }
        public void PropertiesProfileOpen(User user)
        {
            PropertiesPage.GetInstance().Load();
            MainFrame.Navigate(PropertiesPage.GetInstance());
        }
        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => GetInstance().PropertiesProfileOpen(client.GetMe());

        public void ExitFromAccount()
        {
            client = null;
            ChatListGrid.Children.Clear();
            FriendListGrid.Children.Clear();
            ChatHeader.Children.Clear();
            ChatMessagesGrid.Children.Clear();
            MyAvatar.ImageSource = null;
            MyUsername.Text = null;
            JsonTokenWorker.SetToken(null);
            _instance = null;
            AuthorizationPage.GetInstance().OpenAuth();
            MainWindow.GetInstance().MainFrame.Navigate(AuthorizationPage.GetInstance());
        }

        private void ClipMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ChatHeader.Children.Count == 0) return;
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg",
                Multiselect = false
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;

            if (!openFileDialog.CheckFileExists) return;
            var userId = ((UserItem)ChatHeader.Children[0]).user.UserId;
            client.SendPrivateMessage(userId, new PrivateMessageModel
            {
                Text = MessageTextBox.Text,
                Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
            });
            MessageTextBox.Clear();
            LoadPrivateChat(userId);
        }
    }
}