using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
            if (message.Author.Base64Avatar is not null)
                Avatar.ImageSource = ToImage(Convert.FromBase64String(message.Author.Base64Avatar));
            CreatedTime.Content = message.CreateTime.ToShortDateString() + " " + message.CreateTime.ToShortTimeString();
            Username.Content = message.Author.Username;
            Text.Content = message.Text;
            if (message.Base64Image is null)
            {
                HeightItem = 90d;
                ImageRow.Height = new GridLength(0d);
            }
            else
            {
                HeightItem = 290d;
                Image.Source = ToImage(Convert.FromBase64String(message.Base64Image));
            }
        }

        private static BitmapImage ToImage(byte[] array)
        {
            if (array is null) return null;
            using var ms = new System.IO.MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
    }
}
