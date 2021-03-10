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
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;

namespace Vardone.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        private static AuthorizationPage _instance;
        public static AuthorizationPage GetInstance() => _instance ??= new AuthorizationPage();
        private AuthorizationPage() => InitializeComponent();

        public void OpenRegistration()
        {
            Auth.Visibility = Visibility.Hidden;
            Register.Visibility = Visibility.Visible;
        }

        public void OpenAuth()
        {
            Auth.Visibility = Visibility.Visible;
            Register.Visibility = Visibility.Hidden;
        }

        public bool TryLogin(string email, string password)
        {
            var token = VardoneBaseApi.GetUserToken(email, password);
            if (token is null) return false;
            MainWindow.GetInstance().LoadApp(new VardoneClient(token.UserId, token.Token));
            return true;
        }

        public bool TryRegister(string username, string email, string password)
        {
            if (VardoneBaseApi.RegisterUser(new RegisterUserModel
            { Email = email, Password = password, Username = username }) is false)
            {
                return false;
            }

            var token = VardoneBaseApi.GetUserToken(email, password);
            if (token is null) return false;
            MainWindow.GetInstance().LoadApp(new VardoneClient(token.UserId, token.Token));
            return true;
        }
    }
}
