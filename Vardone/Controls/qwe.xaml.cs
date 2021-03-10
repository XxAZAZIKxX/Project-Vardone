using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Vardone.Controls;
using Vardone.Controls.Profile;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for qwe.xaml
    /// </summary>
    public partial class qwe : Page
    {
        public static string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
        public static string directory = System.IO.Path.GetDirectoryName(path);
        private static qwe _instance;
        public static qwe GetInstance() => _instance ??= new qwe();
        private qwe()
        {
            InitializeComponent();

            ServersAvatar my = new ServersAvatar();
            my.image = ToImage(File.ReadAllBytes(@directory + @"\Resources\gear.png"));

            // DataGSA.Items.Add(my);
            FriendsGrid.Children.Add(new friend());

        }
        public BitmapImage ToImage(byte[] array)
        {
            using var ms = new System.IO.MemoryStream(array);
            var image = new BitmapImage();
            image.BeginInit();
            image.CacheOption = BitmapCacheOption.OnLoad; // here
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        public class ServersAvatar
        {
            public BitmapImage image { get; set; }
        }
        private void UserProfileOpen(object s, MouseEventArgs e)
        {
            FrameUserProfile.Navigate(UserProfile.GetInstance());
        }
    }
}
