using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vardone.Core;

namespace Vardone.Pages.Popup
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

        private void DownloadImageMouseDown(object sender, MouseButtonEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg", FileName = "image"
            };
            var dialogResult = saveFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!saveFileDialog.CheckPathExists) return;
            var stream = saveFileDialog.OpenFile();
            stream.Write(ImageWorker.BitmapImageToBytes(image));
            stream.Close();
        }
    }
}