using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Notification.Wpf;
using Notifications.Wpf;
using Vardone.Pages;
using Vardone.Pages.Popup;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for GuildChannelItem.xaml
    /// </summary>
    public partial class GuildChannelItem
    {
        public Channel Channel { get; private set; }
        public enum ActiveContextMenu { Active, Disable }
        public GuildChannelItem([NotNull] Channel channel, ActiveContextMenu activeContextMenu = ActiveContextMenu.Active)
        {
            InitializeComponent();
            Channel = channel;
            ChannelName.Text = Channel.Name;
            ContextMenu.Visibility = activeContextMenu switch
            {
                ActiveContextMenu.Active => Visibility.Visible,
                ActiveContextMenu.Disable => Visibility.Collapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(activeContextMenu), activeContextMenu, null)
            };
        }

        private void OpenChannel(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().chatControl.LoadChat(Channel);

        public void UpdateChannel(Channel channel)
        {
            Channel = channel;
            ChannelName.Text = channel.Name;
        }

        private void MenuItemEditChannelButtonClicked(object sender, RoutedEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(EditChannelNamePage.GetInstance().Load(Channel, EditChannelNamePage.ActionType.Edit));
        private void MenuItemDeleteChannelButtonClicked(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы действительно хотите удалить канал \"{Channel.Name}\"?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            try
            {
                MainPage.Client.DeleteChannel(Channel.ChannelId);
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Success,
                    Title = "Успех",
                    Message = $"Канал \"{Channel.Name}\" был успешно удален"
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
            if (ChatControl.GetInstance().Channel?.ChannelId == Channel.ChannelId) ChatControl.GetInstance().CloseChat();
        }
    }
}
