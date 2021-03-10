using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Vardone.Pages;

namespace Vardone.Controls
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register
    {
        public Register() => InitializeComponent();

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

            if (AuthorizationPage.GetInstance().TryRegister(TbLogin.Text, TbEmail.Text, PbPassword.Password) is false)
            {
                MessageBox.Show("Ошибка регистрации", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
    }
}
