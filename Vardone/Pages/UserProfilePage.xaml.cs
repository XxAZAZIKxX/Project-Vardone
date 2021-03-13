using System;
using System.Windows.Input;
using Vardone.Core;
using VardoneEntities.Entities;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for UserProfilePage.xaml
    /// </summary>
    public partial class UserProfilePage
    {
        private static UserProfilePage _instance;
        public static UserProfilePage GetInstance() => _instance ??= new UserProfilePage();
        public User User { get; private set; }
        private UserProfilePage() => InitializeComponent();
        public void Load(User user)
        {
            User = user;
            Username.Text = user.Username;
            Avatar.ImageSource = user.Base64Avatar == null
                ? MainPage.DefaultAvatar
                : ImageWorker.ByteArrayToImage(Convert.FromBase64String(user.Base64Avatar));
            Description.Text = user.Description;
        }
        private void BackToMainPage(object s, MouseEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);
    }
}
