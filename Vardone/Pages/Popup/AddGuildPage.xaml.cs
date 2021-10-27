using System.Windows.Input;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for AddGuildPage.xaml
    /// </summary>
    public partial class AddGuildPage
    {
        private static AddGuildPage _instance;
        public static AddGuildPage GetInstance() => _instance ??= new AddGuildPage();
        private AddGuildPage() => InitializeComponent();

        private void BackToMainPage(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);
    }
}
