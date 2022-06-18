using System.Windows;
using Notification.Wpf;
using Notifications.Wpf;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;
using Vardone.Controls.Items;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for BannedMemberItem.xaml
    /// </summary>
    public partial class BannedMemberItem
    {
        public BannedMember CurrentBannedMember { get; }
        public BannedMemberItem(BannedMember bannedMember)
        {
            InitializeComponent();
            CurrentBannedMember = bannedMember;
            BannedMember.Child = new UserItem(bannedMember.BannedUser, UserItemType.View);
            var reason = bannedMember.Reason is null or "" ? "Причина не указана" : bannedMember.Reason;
            if (reason.Length > 17)
            {
                BannedByReasonTb.Content = reason[..17] + "..";
                BannedByReasonTb.ToolTip = reason;
            }
            else BannedByReasonTb.Content = reason;
            BannedTime.Content = bannedMember.BanDateTime.ToLongDateString();
            WasBannedByMember.Child = new UserItem(bannedMember.BannedByUser, UserItemType.View);
        }

        private void UnbanButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.Client.UnbanMember(CurrentBannedMember.BannedUser.UserId, CurrentBannedMember.Guild.GuildId);
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
        }

        private void BannedMember_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GuildMembersPage.GetInstance().UserProfileOpen(CurrentBannedMember.BannedUser, MainPage.Client.GetOnlineUser(CurrentBannedMember.BannedUser.UserId));
            MemberItem.isMemberProfileOpen = true;
        }

        private void WasBannedByMember_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            GuildMembersPage.GetInstance().UserProfileOpen(CurrentBannedMember.BannedByUser, MainPage.Client.GetOnlineUser(CurrentBannedMember.BannedByUser.UserId));
            MemberItem.isMemberProfileOpen = true;
        }
    }
}
