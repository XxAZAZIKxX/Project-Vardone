using System.Linq;
using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Controls.ItemControls;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for FriendsProperties.xaml
    /// </summary>
    public partial class FriendsProperties
    {
        private static FriendsProperties _instance;
        public static FriendsProperties GetInstance() => _instance ??= new FriendsProperties();

        private FriendsProperties() => InitializeComponent();

        private void CloseMouseDown(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            if (@TbFriendName.Text.Trim() == MainPage.client.GetMe().Username)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Некорректное имя пользователя",
                    Message = "Нельзя добавить себя в друзья",
                    Type = NotificationType.Error
                });
                return;
            }
            try
            {
                MainPage.client.AddFriend(@TbFriendName.Text.Trim());
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Успех",
                    Message = "Заявка была успешно отправлена",
                    Type = NotificationType.Success
                });
                TbFriendName.Text = "";
            }
            catch
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Некорректное имя пользователя",
                    Message = "Такого пользователя не существует",
                    Type = NotificationType.Error
                });
            }
        }

        public void LoadIncomingRequests()
        {
            var requests = MainPage.client.GetIncomingFriendRequests();
            foreach (var friendRequestItem in requests.OrderBy(p=>p.Username).Select(user => new FriendRequestItem(user)))
            {
                IncomingRequest.Children.Add(friendRequestItem);
            }
        }
    }
}