using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Controls;
using Vardone.Controls.Items;
using VardoneEntities.Entities.Guild;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Interaction logic for GuildMembersPage.xaml
    /// </summary>
    public partial class GuildMembersPage
    {
        private static GuildMembersPage _instance;
        public static GuildMembersPage GetInstance() => _instance ??= new GuildMembersPage();
        public static void ClearInstance() => _instance = null;
        private GuildMembersPage() => InitializeComponent();

        private Guild _guild;

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            Clear();
        }

        private void Clear()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                InviteCodeTb.Text = "Нажмите на кнопку";
                GuildMembersList.Children.Clear();
                BannedMembersList.Children.Clear();
                CreateInviteTab.IsSelected = true;
            });
            _guild = null;
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
            UpdateInvites();
        }

        public GuildMembersPage Load(Guild guild)
        {
            _guild = guild;
            if (guild.Owner.User.UserId == MainPage.Client.GetMe().UserId)
            {
                InvintationsTab.Visibility = BannedMembersTab.Visibility = Visibility.Visible;
            }
            else
            {
                InvintationsTab.Visibility = BannedMembersTab.Visibility = Visibility.Collapsed;
            }
            UpdateMembers();
            UpdateBannedMembers();
            UpdateInvites();
            return this;
        }

        public void UpdateInvites()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                InviteList.Children.Clear();
                if (_guild?.Owner.User.UserId != MainPage.Client.GetMe().UserId) return;
                foreach (var guildInvite in MainPage.Client.GetGuildInvites(_guild.GuildId))
                {
                    InviteList.Children.Add(new GuildInvitationItem(guildInvite));
                }
            });
        }

        public void UpdateMembers()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GuildMembersList.Children.Clear();
                var currentUserId = MainPage.Client.GetMe().UserId;
                foreach (var guildMember in MainPage.Client.GetGuildMembers(_guild.GuildId))
                {
                    var viewPermission = _guild.Owner.User.UserId == currentUserId && guildMember.User.UserId != currentUserId ? MemberItem.ViewPermission.Owner : MemberItem.ViewPermission.Member;
                    GuildMembersList.Children.Add(new MemberItem(guildMember, viewPermission));
                }
            });
        }

        public void UpdateBannedMembers()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                BannedMembersList.Children.Clear();
                if (_guild?.Owner.User.UserId != MainPage.Client.GetMe().UserId) return;
                foreach (var bannedGuildMember in MainPage.Client.GetBannedGuildMembers(_guild.GuildId))
                {
                    BannedMembersList.Children.Add(new BannedMemberItem(bannedGuildMember));
                }
            });
        }
    }
}
