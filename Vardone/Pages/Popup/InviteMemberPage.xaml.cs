using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Controls;
using Vardone.Controls.ItemControls;
using VardoneEntities.Entities.Guild;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for InviteMemberPage.xaml
    /// </summary>
    public partial class InviteMemberPage
    {
        private static InviteMemberPage _instance;
        public static InviteMemberPage GetInstance() => _instance ??= new InviteMemberPage();
        public static void ClearInstance() => _instance = null;
        private InviteMemberPage() => InitializeComponent();

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            Clear();
        }

        private void Clear()
        {
            InviteCodeTb.Text = "Нажмите на кнопку";
        }

        private void GenerateButtonClicked(object sender, RoutedEventArgs e)
        {
            var guildInvite = MainPage.Client.CreateGuildInvite(GuildPanelControl.GetInstance().currentGuild.GuildId);
            InviteCodeTb.Text = guildInvite.InviteCode;
            Clipboard.SetText(guildInvite.InviteCode);
            MainWindow.GetInstance().notificationManager.Show(new NotificationContent
            {
                Title = "Успех",
                Message = "Код приглашения был скопирован",
                Type = NotificationType.Success
            });
        }

        public InviteMemberPage Load(Guild guild)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildMembersList.Children.Clear();
                InviteList.Children.Clear();
                if (guild.Owner.UserId != MainPage.Client.GetMe().UserId)
                {
                    Invintations.Visibility = Visibility.Collapsed;
                }
                else
                {
                    Invintations.Visibility = Visibility.Visible;
                    foreach (var guildInvite in MainPage.Client.GetGuildInvites(guild.GuildId))
                    {
                        InviteList.Children.Add(new InvitationItem(guildInvite));
                    }
                }
                foreach (var guildMember in MainPage.Client.GetGuildMembers(guild.GuildId))
                {
                    GuildMembersList.Children.Add(new UserItem(guildMember, UserItemType.View));
                }
            });
            return this;
        }
    }
}
