using System;
using System.Windows;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    public enum RequestType
    {
        Incoming,
        Outgoing
    }

    /// <summary>
    /// Interaction logic for FriendRequestItem.xaml
    /// </summary>
    public partial class FriendRequestItem
    {
        public User user;
        public FriendRequestItem(User user, RequestType type)
        {
            InitializeComponent();
            this.user = user;

            switch (type)
            {
                case RequestType.Incoming:
                    break;
                case RequestType.Outgoing:
                    Accept.Visibility = Visibility.Hidden;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            if (!MainPage.UserAvatars.ContainsKey(user.UserId))
                MainPage.UserAvatars.Add(user.UserId, user.Base64Avatar switch
                {
                    null => MainPage.DefaultAvatar,
                    _ => ImageWorker.BytesToBitmapImage(Convert.FromBase64String(user.Base64Avatar))
                });
            Avatar.ImageSource = MainPage.UserAvatars[user.UserId];

            Username.Content = user.Username;
        }

        private void Accept_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.client.AddFriend(user.Username);
            FriendsProperties.GetInstance().LoadIncomingRequests();
        }

        private void Decline_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.client.DeleteFriend(user.UserId);
            FriendsProperties.GetInstance().LoadIncomingRequests();
            FriendsProperties.GetInstance().LoadOutgoingRequests();
        }
    }
}
