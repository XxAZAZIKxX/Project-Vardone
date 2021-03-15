using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Notifications.Wpf;
using Vardone.Pages;
using Visibility = System.Windows.Visibility;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for AuthControl.xaml
    /// </summary>
    public partial class AuthControl
    {
        public AuthControl() => InitializeComponent();

        private void Hlme(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            Hyplin.Foreground = (Brush)bc.ConvertFrom("#8f34eb");
        }
        private void Hlml(object sender, MouseEventArgs e)
        {
            var bc = new BrushConverter();
            Hyplin.Foreground = (Brush)bc.ConvertFrom("#34ebe5");
        }
        private void md_hl(object sender, RoutedEventArgs e) => AuthorizationPage.GetInstance().OpenRegistration();

        private void LoginBtnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TblEmail.Text))
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Пожалуйста введите email",
                    Type = NotificationType.Warning
                });
                return;
            }

            if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Пожалуйста введите пароль",
                    Type = NotificationType.Warning
                });
                return;
            }

            if (AuthorizationPage.GetInstance().TryLogin(TblEmail.Text, PbPassword.Password) is false)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Попытка входа завершилась неудачно",
                    Type = NotificationType.Error
                });
                return;
            }

            TblEmail.Text = string.Empty;
            TblEmail_OnLostFocus(null, null);
            PbPassword.Password = string.Empty;
            PbPassword_OnLostFocus(null, null);
        }

        private void TblEmail_OnGotFocus(object sender, RoutedEventArgs e) => EmailPlaceholder.Visibility = Visibility.Collapsed;

        private void TblEmail_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TblEmail.Text)) return;
            EmailPlaceholder.Visibility = Visibility.Visible;
        }

        private void TblEmail_OnTextChanged(object sender, TextChangedEventArgs e) => TblEmail_OnGotFocus(null, null);

        private void PbPassword_OnGotFocus(object sender, RoutedEventArgs e) => PasswordPlaceholder.Visibility = Visibility.Collapsed;

        private void PbPassword_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PbPassword.Password)) return;
            PasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void PbPassword_OnPasswordChanged(object sender, RoutedEventArgs e) => PbPassword_OnGotFocus(null, null);

        private void PbPassword_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) LoginBtnClick(null, null);
        }
    }
}
