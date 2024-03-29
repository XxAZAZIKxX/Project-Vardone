﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Vardone.Core;
using Vardone.Pages.PropertyPages;
using Vardone.Controls.Items;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.User;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for UserProfilePage.xaml
    /// </summary>
    public partial class UserProfilePage
    {
        private static UserProfilePage _instance;
        public static UserProfilePage GetInstance() => _instance ??= new UserProfilePage();
        public static void ClearInstance() => _instance = null;

        private User User { get; set; }
        private UserProfilePage() => InitializeComponent();
        public UserProfilePage Load(User user, bool online, bool isMe = false)
        {
            User = user;
            Username.Text = user.Username;
            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
            Description.Text = user.Description;
            Message.Visibility = isMe ? Visibility.Hidden : Visibility.Visible;

            OnlineStatus.Fill = online switch
            {
                true => new SolidColorBrush(Colors.LimeGreen),
                false => new SolidColorBrush(Color.FromRgb(80, 80, 80))
            };
            OnlineText.Text = online switch
            {
                true => "в сети",
                false => "не в сети"
            };
            Post.Text = user.AdditionalInformation?.Position;
            return this;
        }
        private void BackToMainPage(object s, MouseEventArgs e)
        {
            if (MemberItem.isMemberProfileOpen)
            {
                GuildMembersPage.GetInstance().Frame.Navigate(null);
            }
            else MainPage.GetInstance().MainFrame.Navigate(null);
        }

        private void Message_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (User.UserId != MainPage.Client.GetMe().UserId)
            {
                MainPage.GetInstance().chatControl.LoadChat(new PrivateChat { ToUser = User });
                MainPage.GetInstance().MainFrame.Navigate(null);
            }
        }
    }
}
