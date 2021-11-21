using System;
using System.Windows;
using Notifications.Wpf;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for BannedMemberItem.xaml
    /// </summary>
    public partial class BannedMemberItem
    {
        private readonly BannedMember _member;
        public BannedMemberItem(BannedMember member)
        {
            InitializeComponent();
            _member = member;
            BannedMember.Child = new UserItem(member.BannedUser, UserItemType.View);
            var reason = member.Reason is null or "" ? "Причина не указана" : member.Reason;
            if (reason.Length > 17)
            {
                BannedByReasonTb.Content = reason[..17] + "..";
                BannedByReasonTb.ToolTip = reason;
            }
            else BannedByReasonTb.Content = reason;
            BannedTime.Content = member.BanDateTime.ToLongDateString();
            WasBannedByMember.Child = new UserItem(member.BannedByUser, UserItemType.View);
        }

        private void UnbanButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.Client.UnbanMember(_member.BannedUser.UserId, _member.Guild.GuildId);
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
            GuildMembersPage.GetInstance().UpdateBannedMembers();
        }
    }
}
