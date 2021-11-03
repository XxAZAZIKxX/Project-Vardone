using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore.Internal;
using Notifications.Wpf;
using Vardone.Core;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Guilds;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Interaction logic for GuildProperties.xaml
    /// </summary>
    public partial class GuildProperties
    {
        private static GuildProperties _instance;
        public static GuildProperties GetInstance() => _instance ??= new GuildProperties();
        public GuildProperties() => InitializeComponent();
        private Guild _guild;

        private void Change_Avatar(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg", Multiselect = false };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            MainPage.Client.UpdateGuild(new UpdateGuildModel
            {
                GuildId = _guild.GuildId,
                Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
            });
            LoadGuild(_guild);
        }

        private void CloseMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            MainPage.GetInstance().OpenGuild(_guild);
        }

        public void LoadGuild(Guild guild)
        {
            _guild = guild;
            AvatarImage.ImageSource = AvatarsWorker.GetGuildAvatar(guild.GuildId);
            GuildName.Content = guild.Name;
            GuildNameTb.Text = guild.Name;
        }

        private void GuildDeleteButtonClicked(object sender, RoutedEventArgs e)
        {
            MainPage.Client.DeleteGuild(_guild.GuildId);
            MainPage.GetInstance().MainFrame.Navigate(null);
            MainPage.GetInstance().LoadGuilds();
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
                var guild = MainPage.Client.GetGuilds().FirstOr(p=>p.GuildId == _guild.GuildId, null!);
                if(guild is not null) LoadGuild(guild);
                else CloseMouseDown(null, null);
            }
            GuildNameTb.IsEnabled = !GuildNameTb.IsEnabled;
            GuildNameChangeButton.Content = GuildNameChangeButton.Content.ToString() == "Изменить" ? "Сохранить" : "Изменить";
        }
    }
}
