using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    public enum UserItemType
    {
        Chat,
        Friend
    }

    /// <summary>
    ///     Interaction logic for UserItem.xaml
    /// </summary>
    public partial class UserItem
    {
        public User User { get; }
        public UserItemType Type { get; }

        public UserItem([NotNull] User user, UserItemType type)
        {
            InitializeComponent();
            User = user;
            Username.Content = user.Username;
            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
            Type = type;
            switch (Type)
            {
                case UserItemType.Chat:
                    Grid.MouseLeftButtonDown += OpenChat;
                    SecondAction.Header = "Удалить чат";
                    FirstAction.Visibility = Visibility.Collapsed;
                    SecondAction.Click += DeleteChat;
                    break;
                case UserItemType.Friend:
                    Grid.MouseLeftButtonDown += OpenProfile;
                    CmBorder.Visibility = Visibility.Hidden;
                    SecondAction.Click += DeleteFriend;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private void DeleteChat(object sender, RoutedEventArgs e)
        {
            var vardoneClient = MainPage.Client;
            vardoneClient.DeleteChat(vardoneClient.GetPrivateChatWithUser(User.UserId).ChatId);
        }

        public void SetCountMessages(int count)
        {
            if (Type == UserItemType.Friend) return;
            switch (count)
            {
                case < 0: return;
                case 0:
                    CountMessages.Content = 0;
                    CmBorder.Visibility = Visibility.Hidden;
                    break;
                default:
                    CmBorder.Visibility = Visibility.Visible;
                    CountMessages.Content = count;
                    break;
            }
        }

        public void SetStatus(bool online)
        {
            OnlineStatus.Fill = online switch
            {
                true => new SolidColorBrush(Colors.LimeGreen),
                false => new SolidColorBrush(Color.FromRgb(80, 80, 80))
            };
        }

        private void OpenChat(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().LoadPrivateChat(User.UserId);

        private void OpenProfile(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().UserProfileOpen(User, MainPage.Client.GetOnlineUser(User.UserId));

        private void DeleteFriend(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show("Вы действительно хотите удалить друга?", "Подтвердите",
                MessageBoxButton.OKCancel);
            if (messageBoxResult != MessageBoxResult.OK) return;
            MainPage.Client.DeleteFriend(User.UserId);
            MainPage.GetInstance().LoadFriendList();
        }

        private void SendMessage(object sender, RoutedEventArgs e) => MainPage.GetInstance().LoadPrivateChat(User.UserId);
    }
}