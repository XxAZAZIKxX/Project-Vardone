using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Notifications.Wpf;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.User;

namespace Vardone.Controls.Items
{
    public enum UserItemType
    {
        Chat,
        Friend,
        Profile,
        View
    }

    /// <summary>
    ///     Interaction logic for UserItem.xaml
    /// </summary>
    public partial class UserItem
    {
        public User User { get; private set; }
        private UserItemType Type { get; }

        public UserItem(User user, UserItemType type)
        {
            InitializeComponent();
            User = user;
            Username.Content = user.Username;
            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
            Type = type;
            SetStatus(MainPage.Client.GetOnlineUser(user.UserId));
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
                    CountMessagesCircle.Visibility = Visibility.Hidden;
                    SecondAction.Click += DeleteFriend;
                    break;
                case UserItemType.Profile:
                    Grid.MouseLeftButtonDown += OpenProfile;
                    CountMessagesCircle.Visibility = Visibility.Hidden;
                    ContextMenu.Visibility = Visibility.Collapsed;
                    break;
                case UserItemType.View:
                    CountMessagesCircle.Visibility = Visibility.Hidden;
                    ContextMenu.Visibility = Visibility.Collapsed;
                    Grid.Cursor = Cursors.Arrow;
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public void UpdateUser(User user)
        {
            User = user;
            Username.Content = user.Username;
            AvatarsWorker.UpdateAvatarUser(user.UserId);
            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
        }

        public void UpdateUserOnline() => SetStatus(MainPage.Client.GetOnlineUser(User.UserId));

        private void DeleteChat(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы точно хотите удалить чат с {User.Username}?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            var vardoneClient = MainPage.Client;
            try
            {
                vardoneClient.DeleteChat(vardoneClient.GetPrivateChatWithUser(User.UserId).ChatId);
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Success,
                    Title = "Успех",
                    Message = $"Чат с {User.Username} был успешно удален"
                });
            }
            catch
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Error,
                    Title = "Ошибка",
                    Message = "Что-то пошло не так"
                });
            }
        }
        public void SetCountMessages(int count)
        {
            if (Type == UserItemType.Profile) return;
            switch (count)
            {
                case < 0: return;
                case 0:
                    CountMessages.Content = 0;
                    CountMessagesCircle.Visibility = Visibility.Hidden;
                    break;
                default:
                    CountMessagesCircle.Visibility = Visibility.Visible;
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
        private void OpenChat(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().chatControl.LoadChat(MainPage.Client.GetPrivateChatWithUser(User.UserId));
            SetCountMessages(0);
        }

        private void OpenProfile(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().UserProfileOpen(User, MainPage.Client.GetOnlineUser(User.UserId));
        private void DeleteFriend(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы действительно хотите удалить друга? {User.Username}", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            try
            {
                MainPage.Client.DeleteFriend(User.UserId);
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Success,
                    Title = "Успех",
                    Message = $"Друг {User.Username} был успешно удален"
                });
            }
            catch
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Error,
                    Title = "Ошибка",
                    Message = "Что-то пошло не так"
                });
            }
        }
        private void SendMessage(object sender, RoutedEventArgs e) => MainPage.GetInstance().chatControl.LoadChat(new PrivateChat { ToUser = User });
    }
}