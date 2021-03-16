using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using System.Linq;
using VardoneLibrary.Core;
using Vardone.Controls.ItemControls;
using Application = System.Windows.Application;
namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for FriendsProperties.xaml
    /// </summary>
    public partial class FriendsProperties
    {
        private static FriendsProperties _instance;
            public static VardoneClient client;
        public static FriendsProperties GetInstance() => _instance ??= new FriendsProperties();
        public FriendsProperties()
        {
            InitializeComponent();
        }

        private void CloseMouseDown(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.client.AddFriend(@TbFriendName.Text.Trim().ToString());
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
    
    }
}
