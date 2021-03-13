using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
                MessageBox.Show("Пустой логин", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(PbPassword.Password))
            {
                MessageBox.Show("Пустой пароль", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(TbEmail.Text))
            {
                MessageBox.Show("Пустой емейл", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (AuthorizationPage.GetInstance().TryRegister(TbLogin.Text, TbEmail.Text, PbPassword.Password) is not false) return;
            MessageBox.Show("Ошибка регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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