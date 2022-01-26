using System.Windows.Input;
using System.Windows.Media.Imaging;
using Vardone.Controls;
using Vardone.Core;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for ImageMessagePreview.xaml
    /// </summary>
    public partial class ImageMessagePreview
    {
        private static ImageMessagePreview _instance;
        public static ImageMessagePreview GetInstance() => _instance ??= new ImageMessagePreview();

        private BitmapImage _image;

        public ImageMessagePreview() => InitializeComponent();
        public ImageMessagePreview Load(BitmapImage loadImage, string text)
        {
            MessageTextBox.Text = text;
            picture.Source = _image = loadImage;
            return this;
        }
        private void CancelButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            MessageTextBox.Text = "";
            picture.Source = _image = null;
        }
        private void SendButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChatControl.GetInstance().SendMessage(MessageTextBox.Text, ImageWorker.BitmapImageToBytes(_image));
            CancelButtonDown(null, null);
        }

        private void MessageTextKeyUp(object sender, KeyEventArgs e)
        {
            if(e.Key != Key.Enter)return;
            SendButtonDown(null, null);
        }
    }
}
