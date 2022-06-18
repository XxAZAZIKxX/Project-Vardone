using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Notification.Wpf;
using Notifications.Wpf;
using Vardone.Controls.Items;
using Vardone.Pages.Popup;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;

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
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                InviteCodeTb.Text = "Нажмите на кнопку";
                GuildMembersList.Children.Clear();
                BannedMembersList.Children.Clear();
                CreateInviteTab.IsSelected = true;
            }, DispatcherPriority.Background);
            _guild = null;
        }

        private void GenerateButtonClicked(object sender, RoutedEventArgs e)
        {
            var guildInvite = MainPage.Client.CreateGuildInvite(_guild.GuildId);
            InviteCodeTb.Text = guildInvite.InviteCode;
            Clipboard.SetText(guildInvite.InviteCode);
            MainWindow.GetInstance().notificationManager.Show(new NotificationContent
            {
                Title = "Успех",
                Message = "Код приглашения был скопирован",
                Type = NotificationType.Information
            });
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

        public void UpdateMember(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                GuildMembersList.Children.Cast<MemberItem>().FirstOrDefault(p => p.CurrentMember.User.UserId == user.UserId)?.UpdateUser(user);
            }, DispatcherPriority.Background);
        }

        public void UpdateMemberOnline(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                GuildMembersList.Children.Cast<MemberItem>().FirstOrDefault(p => p.CurrentMember.User.UserId == user.UserId)?.UpdateUserOnline();
            }, DispatcherPriority.Background);
        }

        private void UpdateInvites()
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                InviteList.Children.Clear();
                if (_guild?.Owner.User.UserId != MainPage.Client.GetMe().UserId) return;
                foreach (var guildInvite in MainPage.Client.GetGuildInvites(_guild.GuildId))
                {
                    InviteList.Children.Add(new GuildInvitationItem(guildInvite));
                }
            }, DispatcherPriority.Render);
        }

        private void UpdateMembers()
        {
            var currentUserId = MainPage.Client.GetMe().UserId;
            Application.Current.Dispatcher.Invoke(() => GuildMembersList.Children.Clear());
            foreach (var guildMember in MainPage.Client.GetGuildMembers(_guild.GuildId))
            {
                var viewPermission =
                    _guild.Owner.User.UserId == currentUserId && guildMember.User.UserId != currentUserId
                        ? MemberItem.ViewPermission.Owner
                        : MemberItem.ViewPermission.Member;
                Application.Current.Dispatcher.BeginInvoke(() => GuildMembersList.Children.Add(new MemberItem(guildMember, viewPermission)), DispatcherPriority.Render);
            }
        }

        private void UpdateBannedMembers()
        {
            Application.Current.Dispatcher.Invoke(() => BannedMembersList.Children.Clear());
            if (_guild?.Owner.User.UserId != MainPage.Client.GetMe().UserId) return;
            foreach (var bannedGuildMember in MainPage.Client.GetBannedGuildMembers(_guild.GuildId))
            {
                Application.Current.Dispatcher.BeginInvoke(() =>
                        BannedMembersList.Children.Add(new BannedMemberItem(bannedGuildMember)),
                    DispatcherPriority.Render);
            }
        }

        public void AddGuildInvite(GuildInvite invite)
        {
            if (_guild?.GuildId != invite.Guild.GuildId) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                InviteList.Children.Add(new GuildInvitationItem(invite));
            }, DispatcherPriority.Background);
        }

        public void RemoveGuildInvite(GuildInvite invite)
        {
            if (_guild?.GuildId != invite.Guild.GuildId) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var guildInvitationItems = InviteList.Children.Cast<GuildInvitationItem>().ToList();
                var item = guildInvitationItems.FirstOrDefault(p => p.invite.InviteId == invite.InviteId);
                if (item is null) return;
                InviteList.Children.RemoveAt(guildInvitationItems.IndexOf(item));
            }, DispatcherPriority.Background);
        }

        public void AddNewMember(Member member)
        {
            if (_guild?.GuildId != member.Guild.GuildId) return;
            var me = MainPage.Client.GetMe();
            var viewPerm = me.UserId == _guild.Owner.User.UserId && me.UserId != member.User.UserId
                ? MemberItem.ViewPermission.Owner
                : MemberItem.ViewPermission.Member;
            Application.Current.Dispatcher.BeginInvoke(() => GuildMembersList.Children.Add(new MemberItem(member, viewPerm)), DispatcherPriority.Background);
        }

        public void RemoveMember(Member member)
        {
            if (_guild?.GuildId != member.Guild.GuildId) return;

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var memberItems = GuildMembersList.Children.Cast<MemberItem>().ToList();
                var item = memberItems.FirstOrDefault(p => p.CurrentMember.User.UserId == member.User.UserId);
                if (item is null) return;
                GuildMembersList.Children.RemoveAt(memberItems.IndexOf(item));
            }, DispatcherPriority.Background);
        }

        public void AddBannedMember(BannedMember bannedMember)
        {
            if (_guild?.GuildId != bannedMember.Guild.GuildId) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var memberItems = GuildMembersList.Children.Cast<MemberItem>().ToList();
                var item = memberItems.FirstOrDefault(p => p.CurrentMember.User.UserId == bannedMember.BannedUser.UserId);
                if (item is null) return;
                GuildMembersList.Children.RemoveAt(memberItems.IndexOf(item));
            }, DispatcherPriority.Background);
            Application.Current.Dispatcher.BeginInvoke(
                () => BannedMembersList.Children.Add(new BannedMemberItem(bannedMember)),
                DispatcherPriority.Background);
        }

        public void RemoveBannedMember(BannedMember bannedMember)
        {
            if (_guild?.GuildId != bannedMember.Guild.GuildId) return;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var bannedMemberItems = BannedMembersList.Children.Cast<BannedMemberItem>().ToList();
                var item = bannedMemberItems.FirstOrDefault(p => p.CurrentBannedMember.BannedUser.UserId == bannedMember.BannedUser.UserId);
                if (item is null) return;
                BannedMembersList.Children.RemoveAt(bannedMemberItems.IndexOf(item));
            }, DispatcherPriority.Background);
        }
        public void UserProfileOpen(User user, bool online, bool isMe = false) => Frame.Navigate(UserProfilePage.GetInstance().Load(user, online, isMe));
    }
}
