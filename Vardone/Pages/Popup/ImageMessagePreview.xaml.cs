using System;
using System.Collections.Generic;
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
using Vardone.Core;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for ImageMessagePreview.xaml
    /// </summary>
    public partial class ImageMessagePreview : Page
    {
        private static ImageMessagePreview _instance;
        public static ImageMessagePreview GetInstance() => _instance ??= new ImageMessagePreview();
        public static void ClearInstance() => _instance = null;
        public ImageMessagePreview() => InitializeComponent();
        public ImageMessagePreview Load(BitmapImage loadImage, string text)
        {
            SomeText.Text = text;
            picture.Source = loadImage;
            _image = loadImage;
            return this;
        }
        private BitmapImage _image;
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            SomeText.Text = "Подпись..";
        }  
        private void Label_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e)
        {
            ChatControl.GetInstance().SendMessage(SomeText.Text, ImageWorker.BitmapImageToBytes(_image));
            Label_MouseLeftButtonDown(null, null);
        }

       
    }
}
