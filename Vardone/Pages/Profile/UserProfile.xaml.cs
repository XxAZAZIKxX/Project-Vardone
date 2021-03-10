using System.Windows.Controls;
using System.Windows.Input;

namespace Vardone.Pages.Profile
{
    /// <summary>
    /// Interaction logic for UserProfile.xaml
    /// </summary>
    public partial class UserProfile : Page
    {
        private static UserProfile _instance;
        public static UserProfile GetInstance() => _instance ??= new UserProfile();
        private UserProfile()
        {
            InitializeComponent();
        }
        private void backtoqwe(object s, MouseEventArgs e)
        {
            //MainFrame.Navigate(MainPage.GetInstance());
            MainPage.GetInstance().FrameUserProfile.Navigate(null);

        }
    }
}
