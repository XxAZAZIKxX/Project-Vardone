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

namespace Vardone.Controls.Profile
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
            //Frame1.Navigate(qwe.GetInstance());
            qwe.GetInstance().FrameUserProfile.Navigate(null);

        }
    }
}
