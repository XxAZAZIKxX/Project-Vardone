using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Core;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;
using MessageBox = System.Windows.MessageBox;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Interaction logic for GuildPropertiesPage.xaml
    /// </summary>
    public partial class GuildPropertiesPage
    {
        private static GuildPropertiesPage _instance;
        public static GuildPropertiesPage GetInstance() => _instance ??= new GuildPropertiesPage();
        public static void ClearInstance() => _instance = null;
        private GuildPropertiesPage() => InitializeComponent();

        private Guild _guild;
        private void Change_Avatar(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg",
                Multiselect = false,
                CheckFileExists = true,
                Title = "Сменить аватар"
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            MainPage.Client.UpdateGuild(new UpdateGuildModel
            {
                GuildId = _guild.GuildId,
                Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
            });
            LoadGuild(_guild);
            MainPage.GetInstance().OpenGuild(_guild);
        }

        private void CloseMouseDown(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        public GuildPropertiesPage LoadGuild(Guild guild)
        {
            _guild = guild;
            AvatarImage.ImageSource = AvatarsWorker.GetGuildAvatar(guild.GuildId);
            GuildName.Content = guild.Name;
            GuildNameTb.Text = guild.Name;
            return this;
        }

        private void GuildDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы действительно хотите удалить сервер \"{_guild.Name}\"?", "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            try
            {
                MainPage.Client.DeleteGuild(_guild.GuildId);
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Success,
                    Title = "Успех",
                    Message = $"Сервер \"{_guild.Name}\" был успешно удален"
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
            MainPage.GetInstance().MainFrame.Navigate(null);
            MainPage.GetInstance().PrivateChatButtonClicked(null, null);
        }

        private void GuildNameChangeButtonOnClick(object sender, RoutedEventArgs e)
        {
            if (GuildNameChangeButton.Content is "Сохранить")
            {
                if (GuildNameTb.Text.Trim() is "")
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Неверно введены данные",
                        Message = "Название не может быть пустым!",
                        Type = NotificationType.Error
                    });
                    return;
                }
                MainPage.Client.UpdateGuild(new UpdateGuildModel
                {
                    GuildId = _guild.GuildId,
                    Name = GuildNameTb.Text
                });
                var guild = MainPage.Client.GetGuilds().FirstOrDefault(p => p.GuildId == _guild.GuildId);
                if (guild is null) CloseMouseDown(null, null);
                else
                {
                    LoadGuild(guild);
                    MainPage.GetInstance().OpenGuild(_guild);
                }
            }
            GuildNameTb.IsEnabled = !GuildNameTb.IsEnabled;
            GuildNameChangeButton.Content = GuildNameChangeButton.Content.ToString() == "Изменить" ? "Сохранить" : "Изменить";
        }
    }
}
