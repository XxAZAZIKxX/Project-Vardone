using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Vardone.Core;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.User;

namespace Vardone.Controls.Items
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
        public User User { get; }
        private RequestType Type { get; }
        public FriendRequestItem([NotNull] User user, RequestType type)
        {
            InitializeComponent();
            User = user;
            Type = type;
            SetStatus(MainPage.Client.GetOnlineUser(user.UserId));
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


            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);

            Username.Content = user.Username;
        }

        private void Accept_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.Client.AddFriend(User.Username);
            FriendsPropertiesPage.GetInstance().LoadIncomingRequests();
        }

        private void Decline_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.Client.DeleteFriend(User.UserId);
            FriendsPropertiesPage.GetInstance().LoadIncomingRequests();
            FriendsPropertiesPage.GetInstance().LoadOutgoingRequests();
        }

        public void SetStatus(bool online)
        {
            OnlineStatus.Fill = online switch
            {
                true => new SolidColorBrush(Colors.LimeGreen),
                false => new SolidColorBrush(Color.FromRgb(80, 80, 80))
            };
        }
    }
}
