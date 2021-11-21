using System;
using System.Windows;
using System.Windows.Controls;
using Vardone.Pages;
using Vardone.Pages.Popup;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for MemberItem.xaml
    /// </summary>
    public partial class MemberItem
    {
        public Member member;

        public enum ViewPermission
        {
            Member, Owner
        }
        public MemberItem(Member member, ViewPermission viewPermission)
        {
            InitializeComponent();
            this.member = member;
            Member.Child = new UserItem(member.User, UserItemType.View);
            NumberOfInvitedMembers.Content = member.NumberInvitedMembers;
            JoinDate.Content = member.JoinDate.ToLongDateString();
            if (viewPermission == ViewPermission.Member)
            {
                KickMenuItem.Visibility = BanMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        private void WriteMessageButtonClick(object sender, RoutedEventArgs e)
        {
            ChatControl.GetInstance().LoadChat(MainPage.Client.GetPrivateChatWithUser(member.User.UserId));
            MainPage.GetInstance().PrivateChatButtonClicked(null, null);
            MainPage.GetInstance().MainFrame.Navigate(null);
        }

        private void KickMemberButtonClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы точно хотите выгнать пользователя {member.User.Username}?",
                "Подтвердите действие",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (messageBoxResult != MessageBoxResult.Yes) return;
            MainPage.Client.KickGuildMember(member.User.UserId, member.Guild.GuildId);
            GuildMembersPage.GetInstance().UpdateMembers();
        }

        private void BanMemberButtonClick(object sender, RoutedEventArgs e) => GuildMembersPage.GetInstance().Frame.Navigate(BanReasonPage.GetInstance().Load(member));
    }
}
