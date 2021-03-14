using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneLibrary.Core;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        private static MainPage _instance;
        public static MainPage GetInstance() => _instance ??= new MainPage();
        public static VardoneClient _client;
        public static BitmapImage DefaultAvatar { get; private set; }
        public static Dictionary<long, BitmapImage> UserAvatars { get; set; } = new();

        private MainPage()
        {
            InitializeComponent();
            DefaultAvatar = ImageWorker.BytesToBitmapImage(File.ReadAllBytes(MainWindow.PATH + @"\resources\avatar.jpg"));
        }

        public void Load(VardoneClient client)
        {
            _client = client;

            LoadFriendList();
            LoadMessageList();
            LoadMe();
        }

        public void LoadMe()
        {
            var me = _client.GetMe();
            if (!UserAvatars.ContainsKey(me.UserId))
                UserAvatars.Add(me.UserId, me.Base64Avatar is null ? DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(me.Base64Avatar)));

            MyUsername.Text = me.Username;
            MyAvatar.ImageSource = UserAvatars[me.UserId];
        }

        public void LoadMessageList()
        {
            foreach (var friendGridItem in _client.GetPrivateChats()
                .OrderByDescending(p => _client.GetPrivateMessagesFromChat(p.ChatId, 1)[0].CreateTime)
                .Select(chat => new UserItem(chat.ToUser, MouseDownEventLogic.OpenChat)))
            {
                MessageListGrid.Children.Add(friendGridItem);
            }
        }

        public void LoadFriendList()
        {
            foreach (var friendGridItem in _client.GetFriends().OrderBy(p => p.Username).Select(friend => new UserItem(friend, MouseDownEventLogic.OpenProfile)))
            {
                FriendListGrid.Children.Add(friendGridItem);
            }
        }

        public void LoadPrivateChat(long userId)
        {
            ChatMessagesGrid.Children.Clear();
            ChatHeader.Children.Clear();
            var user = _client.GetUser(userId);
            var userItem = new UserItem(user, MouseDownEventLogic.OpenProfile)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            ChatHeader.Children.Add(userItem);
            foreach (var message in _client.GetPrivateMessagesFromChat(_client.GetPrivateChatWithUser(userId).ChatId, 5).OrderBy(p => p.MessageId))
            {
                var messageItem = new MessageChatItem(message);
                ChatMessagesGrid.Children.Add(messageItem);
            }

            ChatScrollViewer.ScrollToEnd();
            ChatScrollViewer.ScrollChanged += (_, _) =>
            {
                if (ChatScrollViewer.VerticalOffset != 0) return;
                if (ChatHeader.Children.Count == 0) return;
                var privateMessagesFromChat = _client.GetPrivateMessagesFromChat(_client.GetPrivateChatWithUser(((UserItem)ChatHeader.Children[0]).user.UserId).ChatId, 5,
                    ((MessageChatItem)ChatMessagesGrid.Children[0]).message.MessageId);
                foreach (var message in privateMessagesFromChat)
                {
                    var messageItem = new MessageChatItem(message);
                    ChatMessagesGrid.Children.Insert(0, messageItem);
                }

                if (privateMessagesFromChat.Count > 0)
                    ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
            };
        }

        private void MyProfileOpen(object s, MouseEventArgs e) => UserProfileOpen(_client.GetMe());

        public void UserProfileOpen(User user)
        {
            UserProfilePage.GetInstance().Load(user);
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
            _client.SendPrivateMessage(user.UserId, new PrivateMessageModel { Text = MessageTextBox.Text });
            MessageTextBox.Text = "";
            MessageBoxLostFocus(null, null);
            LoadPrivateChat(user.UserId);
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) => MainFrame.Navigate(PropertiesPage.GetInstance());
    }
}