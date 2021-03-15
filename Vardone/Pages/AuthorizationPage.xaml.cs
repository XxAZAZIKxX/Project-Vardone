using System;
using System.Windows;
using System.Windows.Controls;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;

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
            MainWindow.GetInstance().LoadApp(new VardoneClient(token.UserId, token.Token));
            return true;
        }

        public bool TryRegister(string username, string email, string password)
        {
            var res = VardoneBaseApi.RegisterUser(new RegisterUserModel {Email = email, Password = password, Username = username});

            var token = VardoneBaseApi.GetUserToken(email, password);
            if (token is null) return false;
            MainWindow.GetInstance().LoadApp(new VardoneClient(token.UserId, token.Token));
            return true;
        }
    }
}
