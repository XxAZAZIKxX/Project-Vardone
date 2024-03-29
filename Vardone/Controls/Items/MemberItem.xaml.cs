﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
        public static bool isMemberProfileOpen;

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
            MainPage.GetInstance().PrivateChatButtonClicked(null, null);
            MainPage.GetInstance().MainFrame.Navigate(null);
            ChatControl.GetInstance().LoadChat(MainPage.Client.GetPrivateChatWithUser(CurrentMember.User.UserId));
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

        private void Member_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(e.LeftButton != MouseButtonState.Pressed) return;
            GuildMembersPage.GetInstance().UserProfileOpen(CurrentMember.User, MainPage.Client.GetOnlineUser(CurrentMember.User.UserId));
            isMemberProfileOpen = true;
        }
    }
}
