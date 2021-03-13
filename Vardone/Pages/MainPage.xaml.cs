using System;
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
        private static VardoneClient _client;
        public static BitmapImage DefaultAvatar { get; private set; }

        private MainPage()
        {
            InitializeComponent();
            DefaultAvatar = ImageWorker.BytesToBitmapImage(File.ReadAllBytes(MainWindow.PATH + @"\resources\avatar.jpg"));
        }

        public void Load(VardoneClient client)
        {
            _client = client;

            foreach (var friendGridItem in _client.GetFriends().Select(friend => new UserItem(friend, MouseDownEventLogic.OpenProfile)))
                FriendsGrid.Children.Add(friendGridItem);
            foreach (var friendGridItem in _client.GetPrivateChats().Select(chat => new UserItem(chat.ToUser, MouseDownEventLogic.OpenChat)))
                MessagesGrid.Children.Add(friendGridItem);

            var me = _client.GetMe();
            MyUsername.Text = me.Username;
            MyAvatar.ImageSource = me.Base64Avatar == null ? DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(me.Base64Avatar));
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
            foreach (var messageItem in _client.GetPrivateMessagesFromChat(_client.GetPrivateChatWithUser(userId).ChatId).Select(message => new MessageChatItem(message)))
                ChatMessagesGrid.Children.Add(messageItem);

            ChatScrollViewer.ScrollToEnd();
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