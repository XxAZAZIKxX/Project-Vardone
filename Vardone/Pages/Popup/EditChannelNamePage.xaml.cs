using System;
using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Controls;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for EditChannelNamePage.xaml
    /// </summary>
    public partial class EditChannelNamePage
    {
        private static EditChannelNamePage _instance;
        public static EditChannelNamePage GetInstance() => _instance ??= new EditChannelNamePage();
        private EditChannelNamePage() => InitializeComponent();

        private ActionType _type;
        private Channel _channel;

        private void BackToMainPage(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            Reset();
        }

        public enum ActionType
        {
            Create, Edit
        }

        public EditChannelNamePage Load(Channel channel, ActionType type)
        {
            Reset();
            this._type = type;
            this._channel = channel;
            if (type == ActionType.Edit) ChannelNameTb.Text = channel.Name;
            return this;
        }

        private void Reset() => ChannelNameTb.Text = string.Empty;

        private void SaveButton(object sender, RoutedEventArgs e)
        {
            if (ChannelNameTb.Text.Trim() == string.Empty)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Error,
                    Title = "Введите корректные данные",
                    Message = "Нужно ввести название канала"
                });
                return;
            }

            try
            {
                switch (_type)
                {
                    case ActionType.Create:
                        MainPage.Client.CreateChannel(_channel.Guild.GuildId, ChannelNameTb.Text.Trim());
                        MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                        {
                            Type = NotificationType.Success,
                            Title = "Успех",
                            Message = "Канал был успешно создан"
                        });
                        break;
                    case ActionType.Edit:
                        MainPage.Client.UpdateChannel(new UpdateChannelModel
                        {
                            ChannelId = _channel.ChannelId,
                            Name = ChannelNameTb.Text.Trim()
                        });
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
            BackToMainPage(null, null);
            Reset();
        }

        private void ChannelNameKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            SaveButton(null, null);
        }
    }
}
