using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Vardone.Pages
{
    /// <summary>
    /// Логика взаимодействия для DeployImage.xaml
    /// </summary>
    public partial class DeployImage
    {
        private static DeployImage _instance;
        public static DeployImage GetInstance() => _instance ??= new DeployImage();
        public BitmapImage image;
        private DeployImage() => InitializeComponent();

        public void LoadImage(BitmapImage loadImage) => Image.Source = image = loadImage;

        private void CloseImage(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);
    }
}
