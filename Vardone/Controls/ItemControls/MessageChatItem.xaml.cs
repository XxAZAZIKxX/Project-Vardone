using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Логика взаимодействия для MessageGridItem.xaml
    /// </summary>
    public partial class MessageChatItem
    {
        public PrivateMessage message;
        public MessageChatItem(PrivateMessage message)
        {
            InitializeComponent();
            this.message = message;
            if (!MainPage.UserAvatars.ContainsKey(message.Author.UserId))
                MainPage.UserAvatars.Add(message.Author.UserId, message.Author.Base64Avatar is null ? MainPage.DefaultAvatar : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(message.Author.Base64Avatar)));

            Avatar.ImageSource = MainPage.UserAvatars[message.Author.UserId];

            CreatedTime.Content = message.CreateTime.ToShortDateString() + " " + message.CreateTime.ToShortTimeString();
            Username.Content = message.Author.Username;
            Text.Content = message.Text;
            if (message.Base64Image is null) ImageRow.Height = new GridLength(0d);
            else Image.Source = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(message.Base64Image));
        }

        private void ImageOnClick(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().DeployImage(Image.Source as BitmapImage);
    }
}
