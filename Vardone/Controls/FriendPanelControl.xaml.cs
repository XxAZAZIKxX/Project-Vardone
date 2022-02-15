using System.Windows.Input;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;

namespace Vardone.Controls
{
    /// <summary>
    ///     Interaction logic for FriendPanelControl.xaml
    /// </summary>
    public partial class FriendPanelControl
    {
        private static FriendPanelControl _instance;
        public static FriendPanelControl GetInstance() => _instance ??= new FriendPanelControl();
        public static void ClearInstance() => _instance = null;
        private FriendPanelControl() => InitializeComponent();

        private void OpenFriendsProperties(object sender, MouseButtonEventArgs mouseButtonEventArgs) => MainPage.GetInstance().MainFrame.Navigate(FriendsPropertiesPage.GetInstance().Load());
    }
}