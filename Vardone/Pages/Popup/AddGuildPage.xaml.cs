using System.Windows;
using System.Windows.Input;
using Vardone.Controls;

namespace Vardone.Pages.Popup
{
    /// <summary>
    /// Interaction logic for AddGuildPage.xaml
    /// </summary>
    public partial class AddGuildPage
    {
        private static AddGuildPage _instance;
        public static AddGuildPage GetInstance() => _instance ??= new AddGuildPage();

        private AddGuildPage()
        {
            InitializeComponent();
            var joinGuildControl = JoinGuildControl.GetInstance();
            JoinGuildGrid.Children.Add(joinGuildControl);
        }

        private void BackToMainPage(object sender, MouseButtonEventArgs e)
        {
            MainPage.GetInstance().MainFrame.Navigate(null);
            Reset();
        }

        private void JoinGuildButtonClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            JoinGuildGrid.Visibility = Visibility.Visible;
            AddGuildGrid.Visibility = Visibility.Collapsed;
        }

        private void CreateGuildButtonClicked(object sender, RoutedEventArgs routedEventArgs)
        {
            MainPage.Client.CreateGuild();
            MainPage.GetInstance().LoadGuilds();
            BackToMainPage(null, null);
        }

        public void Reset()
        {
            AddGuildGrid.Visibility = Visibility.Visible;
            JoinGuildGrid.Visibility = Visibility.Collapsed;
            JoinGuildControl.GetInstance().Reset();
        }
    }
}
