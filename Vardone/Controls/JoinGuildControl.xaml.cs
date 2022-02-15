using System;
using System.Windows;
using Notifications.Wpf;
using Vardone.Pages;
using Vardone.Pages.Popup;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for JoinGuildControl.xaml
    /// </summary>
    public partial class JoinGuildControl
    {
        private static JoinGuildControl _instance;
        public static JoinGuildControl GetInstance() => _instance ??= new JoinGuildControl();
        public static void ClearInstance() => _instance = null;
        private JoinGuildControl() => InitializeComponent();

        public void Reset() => CodeTextBox.Text = string.Empty;

        private void JoinButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.Client.JoinGuild(CodeTextBox.Text);
            }
            catch
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Код приглашения неверен или истекло время действия",
                    Type = NotificationType.Error
                });
                return;
            }
            MainPage.GetInstance().MainFrame.Navigate(null);
            AddGuildPage.GetInstance().Reset();
        }
    }
}
