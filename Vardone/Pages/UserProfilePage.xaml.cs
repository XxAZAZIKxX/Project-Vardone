using System;
using System.Windows;
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
        public void Load(User user, bool isMe = false)
        {
            User = user;
            Username.Text = user.Username;
            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
            Description.Text = user.Description;
            Message.Visibility = isMe ? Visibility.Hidden : Visibility.Visible;
        }
        private void BackToMainPage(object s, MouseEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        private void Message_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().LoadPrivateChat(User.UserId);
            MainPage.GetInstance().MainFrame.Navigate(null);
        }
    }
}
