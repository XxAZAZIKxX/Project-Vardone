using System;
using System.Windows.Controls;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for FriendGridItem.xaml
    /// </summary>
    public partial class FriendGridItem : UserControl
    {
        public User user;
        public FriendGridItem(User user)
        {
            InitializeComponent();
            this.user = user;
            Username.Content = user.Username;
            if (user.Base64Avatar is not null)
            {
                Avatar.ImageSource = Base64ToBitmap.ToImage(Convert.FromBase64String(user.Base64Avatar));
            }
        }

        private void GridClick(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().LoadPrivateChat(user.UserId);
        }
    }
}
