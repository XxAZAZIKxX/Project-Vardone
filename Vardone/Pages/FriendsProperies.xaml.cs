using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vardone.Core;
using Notifications.Wpf;
using VardoneEntities.Models.GeneralModels.Users;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for FriendsProperies.xaml
    /// </summary>
    public partial class FriendsProperies : Page
    {
        private static FriendsProperies _instance;
        public static FriendsProperies GetInstance() => _instance ??= new FriendsProperies();
        public FriendsProperies()
        {
            InitializeComponent();
        }

        private void CloseMouseDown(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.client.AddFriend(Convert.ToInt64(TBFriendName.Text));
                TBFriendName.Text = "";
            }
            catch 
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Не корректное имя пользователя",
                    Message = "Такого пользователя не существует",
                    Type = NotificationType.Error
                });
                return;
            }

        }
    }
}
