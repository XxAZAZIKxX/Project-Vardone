using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Pages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for GuildChannelItem.xaml
    /// </summary>
    public partial class GuildChannelItem
    {
        public Channel channel;
        public enum ActiveContextMenu { Active, Disable }
        public GuildChannelItem([NotNull] Channel channel, ActiveContextMenu activeContextMenu = ActiveContextMenu.Active)
        {
            InitializeComponent();
            this.channel = channel;
            ChannelName.Text = this.channel.Name;
            ContextMenu.Visibility = activeContextMenu switch
            {
                ActiveContextMenu.Active => Visibility.Visible,
                ActiveContextMenu.Disable => Visibility.Collapsed,
                _ => throw new ArgumentOutOfRangeException(nameof(activeContextMenu), activeContextMenu, null)
            };
        }

        private void OpenChannel(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().chatControl.LoadChat(channel);

        private void MenuItemEditChannelButtonClicked(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void MenuItemDeleteChannelButtonClicked(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы действительно хотите удалить канал \"{channel.Name}\"?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            try
            {
                MainPage.Client.DeleteChannel(channel.ChannelId);
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Success,
                    Title = "Успех",
                    Message = $"Канал \"{channel.Name}\" был успешно удален"
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
            GuildPanelControl.GetInstance().UpdateChannelsList();
            if (ChatControl.GetInstance().channel?.ChannelId == channel.ChannelId) ChatControl.GetInstance().CloseChat();
        }
    }
}
