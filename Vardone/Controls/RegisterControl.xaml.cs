using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Notifications.Wpf;
using Vardone.Pages;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for RegisterControl.xaml
    /// </summary>
    public partial class RegisterControl
    {
        public RegisterControl() => InitializeComponent();

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

        private void Hlmd(object sender, RoutedEventArgs e) => AuthorizationPage.GetInstance().OpenAuth();

        private void RegisterBtnClick(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text))
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Пожалуйста введите имя пользователя",
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

            if (string.IsNullOrWhiteSpace(TbEmail.Text))
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Пожалуйста введите email",
                    Type = NotificationType.Warning
                });
                return;
            }

            bool tryRegister;
            try
            {
                tryRegister = AuthorizationPage.GetInstance().TryRegister(TbLogin.Text, TbEmail.Text, PbPassword.Password);
            }
            catch (Exception exception)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = exception.Message,
                    Type = NotificationType.Error
                });
                return;
            }
            if (tryRegister is false)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Попытка регистрации завершилась неудачно",
                    Type = NotificationType.Error
                });
                return;
            }

            TbLogin.Text = string.Empty;
            TbLogin_OnLostFocus(null, null);
            TbEmail.Text = string.Empty;
            TbEmail_OnLostFocus(null, null);
            PbPassword.Password = string.Empty;
            PbPassword_OnLostFocus(null, null);
        }

        private void TbLogin_OnGotFocus(object sender, RoutedEventArgs e) => TbLoginPlaceholder.Visibility = Visibility.Collapsed;

        private void TbLogin_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TbLogin.Text)) return;
            TbLoginPlaceholder.Visibility = Visibility.Visible;
        }

        private void TbLogin_OnTextChanged(object sender, TextChangedEventArgs e) => TbLogin_OnGotFocus(null, null);

        private void TbEmail_OnGotFocus(object sender, RoutedEventArgs e) => TbEmailPlaceholder.Visibility = Visibility.Collapsed;

        private void TbEmail_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(TbEmail.Text)) return;
            TbEmailPlaceholder.Visibility = Visibility.Visible;
        }

        private void TbEmail_OnTextChanged(object sender, TextChangedEventArgs e) => TbEmail_OnGotFocus(null, null);

        private void PbPassword_OnGotFocus(object sender, RoutedEventArgs e) => PbPasswordPlaceholder.Visibility = Visibility.Collapsed;

        private void PbPassword_OnLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(PbPassword.Password)) return;
            PbPasswordPlaceholder.Visibility = Visibility.Visible;
        }

        private void PbPassword_OnPasswordChanged(object sender, RoutedEventArgs e) => PbPassword_OnGotFocus(null, null);

        private void PbPassword_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) RegisterBtnClick(null, null);
        }
    }
}