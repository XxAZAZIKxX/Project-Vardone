using System;
using System.IO;
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
            DefaultAvatar = ImageWorker.ByteArrayToImage(File.ReadAllBytes(MainWindow.PATH + @"\resources\avatar.jpg"));
        }

        public void Load(VardoneClient client)
        {
            _client = client;
            var i = 0;
            foreach (var friend in _client.GetFriends())
            {
                var friendGridItem = new UserItem(friend, MouseDownEventLogic.OpenProfile)
                {
                    Margin = new Thickness(0, i, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                FriendsGrid.Children.Add(friendGridItem);
                i = (int)(friendGridItem.Height + friendGridItem.Margin.Top);
            }

            i = 0;
            foreach (var chat in _client.GetPrivateChats())
            {
                var friendGridItem = new UserItem(chat.ToUser, MouseDownEventLogic.OpenChat)
                {
                    Margin = new Thickness(0, i, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                MessagesGrid.Children.Add(friendGridItem);
                i = (int)(friendGridItem.Height + friendGridItem.Margin.Top);
            }
            //client.UpdateUser(new UpdateUserModel
            //{
            //    Username = @"r\\Kzenta",
            //    //Base64Image = Convert.ToBase64String(File.ReadAllBytes(@"D:\User\Pictures\152403578_223907286105845_277220794561786358_n.jpg"))
            //});

            var me = _client.GetMe();
            MyUsername.Text = me.Username;
            MyAvatar.ImageSource = me.Base64Avatar == null
                ? DefaultAvatar
                : ImageWorker.ByteArrayToImage(Convert.FromBase64String(me.Base64Avatar));
        }

        public void LoadPrivateChat(long userId)
        {
            var i = 40;
            ChatMessagesGrid.Children.Clear();
            ChatHeader.Children.Clear();
            var user = _client.GetUser(userId);
            var userItem = new UserItem(user, MouseDownEventLogic.OpenProfile)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Left
            };
            ChatHeader.Children.Add(userItem);
            foreach (var message in _client.GetPrivateMessagesFromChat(_client.GetPrivateChatWithUser(userId).ChatId))
            {
                var messageItem = new MessageChatItem(message)
                {
                    Margin = new Thickness(0, i, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top
                };
                ChatMessagesGrid.Children.Add(messageItem);
                i = (int)(messageItem.HeightItem + messageItem.Margin.Top);
            }

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

        private void TextBox_GotFocus(object sender, RoutedEventArgs e) =>
            MessagePlaceholder.Visibility = Visibility.Collapsed;

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is not TextBox) return;
            var textBox = (TextBox)sender;
            if (!string.IsNullOrEmpty(textBox.Text)) return;
            MessagePlaceholder.Visibility = Visibility.Visible;
        }

        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (ChatHeader.Children.Count == 0) return;
            if (ChatHeader.Children[0] is not UserItem) return;
            var user = ((UserItem)ChatHeader.Children[0]).user;
            _client.SendPrivateMessage(user.UserId, new PrivateMessageModel { Text = MessageTextBox.Text });
            MessageTextBox.Text = "";
            TextBox_LostFocus(MessageTextBox, null);
            LoadPrivateChat(user.UserId);
        }

        private void MessageTextBox_OnTextChanged(object sender, TextChangedEventArgs e) => TextBox_GotFocus(null, null);
    }
}