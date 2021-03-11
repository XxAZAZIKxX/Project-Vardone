using System;
using System.Windows.Controls;
using System.Windows.Input;
using Vardone.Core;
using VardoneEntities.Entities;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Page
    {
        private static UserProfile _instance;
        public static UserProfile GetInstance() => _instance ??= new UserProfile();
        public User User { get; private set; }
        private UserProfile() => InitializeComponent();
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
