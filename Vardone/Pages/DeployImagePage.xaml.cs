using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Vardone.Pages
{
    /// <summary>
    /// Логика взаимодействия для DeployImagePage.xaml
    /// </summary>
    public partial class DeployImagePage
    {
        private static DeployImagePage _instance;
        public static DeployImagePage GetInstance() => _instance ??= new DeployImagePage();
        public BitmapImage image;
        private DeployImagePage() => InitializeComponent();

        public void LoadImage(BitmapImage loadImage) => Image.Source = image = loadImage;

        private void CloseImage(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);
    }
}
