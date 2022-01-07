using System.Windows;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Client;
using VardoneLibrary.Core.Client.Base;

namespace Vardone.Pages
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage
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
            MainWindow.GetInstance().LoadApp(new VardoneClient(token));
            return true;
        }
        public bool TryRegister(string username, string email, string password)
        {
            var registerUser = VardoneBaseApi.RegisterUser(new RegisterUserModel { Email = email, PasswordHash = password, Username = username });
            if (registerUser is false) return false;
            var token = VardoneBaseApi.GetUserToken(email, password);
            if (token is null) return false;
            MainWindow.GetInstance().LoadApp(new VardoneClient(token));
            return true;
        }
    }
}
