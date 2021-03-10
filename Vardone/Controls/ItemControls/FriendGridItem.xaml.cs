using System;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
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
                Avatar.ImageSource = ToImage(Convert.FromBase64String(user.Base64Avatar));
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
