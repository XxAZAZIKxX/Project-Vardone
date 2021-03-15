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

            if (!MainPage.UserAvatars.ContainsKey(user.UserId))
                MainPage.UserAvatars.Add(user.UserId, user.Base64Avatar switch
                {
                    null => MainPage.DefaultAvatar,
                    _ => ImageWorker.BytesToBitmapImage(Convert.FromBase64String(user.Base64Avatar))
                });
            Avatar.ImageSource = MainPage.UserAvatars[user.UserId];


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
