using System;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    public enum MouseDownEventLogic
    {
        OpenChat,
        OpenProfile
    }
    /// <summary>
    /// Interaction logic for UserItem.xaml
    /// </summary>
    public partial class UserItem
    {
        public User user;
        public MouseDownEventLogic ClickLogic { get; set; }
        public UserItem(User user, MouseDownEventLogic logic)
        {
            InitializeComponent();
            this.user = user;
            Username.Content = user.Username;
            Avatar.ImageSource = user.Base64Avatar is not null
                ? ImageWorker.ByteArrayToImage(Convert.FromBase64String(user.Base64Avatar))
                : MainPage.DefaultAvatar;
            ClickLogic = logic;
            Grid.MouseDown += ClickLogic switch
            {
                MouseDownEventLogic.OpenChat => OpenChat,
                MouseDownEventLogic.OpenProfile => OpenProfile,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private void OpenChat(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().LoadPrivateChat(user.UserId);
        private void OpenProfile(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().UserProfileOpen(user);
    }
}
