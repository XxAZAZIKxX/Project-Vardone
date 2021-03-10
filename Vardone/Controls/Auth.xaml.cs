using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Vardone.Pages;
using Brush = System.Windows.Media.Brush;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for Auth.xaml
    /// </summary>
    public partial class Auth
    {
        public Auth() => InitializeComponent();

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

            if (AuthorizationPage.GetInstance().TryLogin(TblEmail.Text, PbPassword.Password) is false)
            {
                MessageBox.Show("Ошибка авторизации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
