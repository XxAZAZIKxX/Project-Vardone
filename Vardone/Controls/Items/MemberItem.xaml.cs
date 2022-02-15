using System;
using System.Windows;
using System.Windows.Controls;
using Vardone.Pages;
using Vardone.Pages.Popup;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for MemberItem.xaml
    /// </summary>
    public partial class MemberItem
    {
        public Member CurrentMember { get; private set; }

        public enum ViewPermission
        {
            Member, Owner
        }
        public MemberItem(Member currentMember, ViewPermission viewPermission)
        {
            InitializeComponent();
            CurrentMember = currentMember;
            Member.Child = new UserItem(currentMember.User, UserItemType.View);
            NumberOfInvitedMembers.Content = currentMember.NumberInvitedMembers;
            JoinDate.Content = currentMember.JoinDate.ToLongDateString();
            if (viewPermission == ViewPermission.Member)
            {
                KickMenuItem.Visibility = BanMenuItem.Visibility = Visibility.Collapsed;
            }
        }

        public void UpdateUser(User user)
        {
            CurrentMember.User = user;
            ((UserItem)Member.Child).UpdateUser(user);
        }

        public void UpdateUserOnline() => ((UserItem)Member.Child).UpdateUserOnline();

        private void WriteMessageButtonClick(object sender, RoutedEventArgs e)
        {
            ChatControl.GetInstance().LoadChat(MainPage.Client.GetPrivateChatWithUser(CurrentMember.User.UserId));
            MainPage.GetInstance().PrivateChatButtonClicked(null, null);
            MainPage.GetInstance().MainFrame.Navigate(null);
        }

        private void KickMemberButtonClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show($"Вы точно хотите выгнать пользователя {CurrentMember.User.Username}?",
                "Подтвердите действие",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (messageBoxResult != MessageBoxResult.Yes) return;
            MainPage.Client.KickGuildMember(CurrentMember.User.UserId, CurrentMember.Guild.GuildId);
        }

        private void BanMemberButtonClick(object sender, RoutedEventArgs e) => GuildMembersPage.GetInstance().Frame.Navigate(BanReasonPage.GetInstance().Load(CurrentMember));
    }
}
