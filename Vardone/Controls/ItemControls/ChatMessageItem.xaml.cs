using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Логика взаимодействия для MessageGridItem.xaml
    /// </summary>
    public partial class ChatMessageItem
    {
        public PrivateMessage Message { get; }
        public User Author { get; }
        public ChatMessageItem(PrivateMessage message)
        {
            InitializeComponent();

            Message = message;
            Author = message.Author;

            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(Author.UserId);

            CreatedTime.Content = message.CreateTime.ToShortDateString() + " " + message.CreateTime.ToShortTimeString();
            Username.Content = Author.Username;
            Text.Content = message.Text;
            if (message.Base64Image is null) ImageRow.Height = new GridLength(0d);
            else Image.Source = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(message.Base64Image));
        }

        private void ImageOnClick(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().DeployImage(Image.Source as BitmapImage);

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
