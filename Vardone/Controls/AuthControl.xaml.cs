using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
                MessageBox.Show("Пустой емейл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show("Пустой пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AuthorizationPage.GetInstance().TryLogin(TblEmail.Text, PbPassword.Password) is not false) return;
            MessageBox.Show("Ошибка авторизации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if(e.Key == Key.Enter) LoginBtnClick(null, null);
        }
    }
}
