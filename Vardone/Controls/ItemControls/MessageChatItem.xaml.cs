using System;
using System.Windows;
using System.Windows.Controls;
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
    public partial class MessageChatItem : UserControl
    {
        public PrivateMessage message;
        public double HeightItem { get; }
        public MessageChatItem(PrivateMessage message)
        {
            InitializeComponent();
            this.message = message;
            Avatar.ImageSource = message.Author.Base64Avatar is not null
                ? ImageWorker.ByteArrayToImage(Convert.FromBase64String(message.Author.Base64Avatar))
                : MainPage.DefaultAvatar;

            CreatedTime.Content = message.CreateTime.ToShortDateString() + " " + message.CreateTime.ToShortTimeString();
            Username.Content = message.Author.Username;
            Text.Content = message.Text;
            if (message.Base64Image is null)
            {
                ImageRow.Height = new GridLength(0d);
                HeightItem = 90d;
            }
            else
            {
                Image.Source = ImageWorker.ByteArrayToImage(Convert.FromBase64String(message.Base64Image));
                HeightItem = 290d;
            }
        }

        private void ImageOnClick(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().DeployImage(Image.Source as BitmapImage);
    }
}
