using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Core;
using VardoneEntities.Models.GeneralModels.Users;
using Application = System.Windows.Application;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Логика взаимодействия для PropertiesPage.xaml
    /// </summary>
    public partial class PropertiesPage
    {
        private static PropertiesPage _instance;
        public static PropertiesPage GetInstance() => _instance ??= new PropertiesPage();
        private PropertiesPage() => InitializeComponent();

        public void Load()
        {
            var user = MainPage.Client.GetMe();
            Application.Current.Dispatcher.Invoke(() =>
            {
                AvatarImage.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
                UsernameLabel.Content = user.Username;
                UsernameTb.Text = user.Username;
                EmailTb.Text = user.Email;
                DescTb.Text = user.Description ?? "Description";
            });
        }
        private void CloseMouseDown(object sender, System.Windows.Input.MouseEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        private void PasswordButton_SaveClick(object sender, RoutedEventArgs e)
        {
            if (ChangeButton.Content is "Сохранить")
            {
                if (string.IsNullOrWhiteSpace(PasswordBox1.Password) || string.IsNullOrWhiteSpace(PasswordBox2.Password))
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Проверьте введенные данные",
                        Message = "Пожалуйста введите пароль",
                        Type = NotificationType.Warning
                    });
                    return;
                }

                try
                {
                    MainPage.Client.UpdatePassword(new UpdatePasswordModel
                    {
                        PreviousPassword = PasswordBox1.Password,
                        NewPassword = PasswordBox2.Password
                    });
                    PasswordButton_CancelClick(null, null);
                    return;
                }
                catch
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Неверный пароль",
                        Message = "Введите корректный пароль",
                        Type = NotificationType.Error
                    });
                    return;
                }
            }

            ChangeButton.Content = ChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            CancelBtn.Visibility = CancelBtn.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            PasswordBox1.Visibility = PasswordBox2.Visibility = PasswordBox1.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PasswordButton_CancelClick(object sender, RoutedEventArgs e)
        {
            ChangeButton.Content = ChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            CancelBtn.Visibility = CancelBtn.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            PasswordBox1.Password = PasswordBox2.Password = string.Empty;
            PasswordBox1.Visibility = PasswordBox2.Visibility = PasswordBox1.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Username_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (UsernameChangeButton.Content is "Сохранить")
            {
                if (string.IsNullOrWhiteSpace(UsernameTb.Text))
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Проверьте введенные данные",
                        Message = "Пожалуйста введите логин",
                        Type = NotificationType.Warning
                    });
                    return;
                }

                MainPage.Client.UpdateMe(new UpdateUserModel
                {
                    Username = UsernameTb.Text
                });
                Load();
            }

            UsernameChangeButton.Content = UsernameChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            UsernameTb.IsEnabled = UsernameChangeButton.Content.ToString() == "Сохранить";
        }

        private void Email_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (EmailChangeButton.Content is "Сохранить")
            {
                if (string.IsNullOrWhiteSpace(EmailTb.Text))
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Title = "Проверьте введенные данные",
                        Message = "Пожалуйста введите email",
                        Type = NotificationType.Warning
                    });
                    return;
                }

                MainPage.Client.UpdateMe(new UpdateUserModel
                {
                    Email = EmailTb.Text
                });
                Load();
            }

            EmailChangeButton.Content = EmailChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            EmailTb.IsEnabled = EmailChangeButton.Content.ToString() == "Сохранить";
        }

        private void Description_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (DescChangeButton.Content.ToString() == "Сохранить")
            {
                MainPage.Client.UpdateMe(new UpdateUserModel
                {
                    Description = DescTb.Text.Trim()
                });
                Load();
            }

            DescChangeButton.Content = DescChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            DescTb.IsEnabled = DescChangeButton.Content.ToString() == "Сохранить";

        }

        private void Change_Avatar(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg", Multiselect = false };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            MainPage.Client.UpdateMe(new UpdateUserModel
            {
                Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
            });
            Load();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            MainPage.Client.CloseCurrentSession();
            MainPage.GetInstance().ExitFromAccount();
        }

        private void ExitEverywhereButtonClick(object sender, RoutedEventArgs e)
        {
            MainPage.Client.CloseAllSessions();
            MainPage.GetInstance().ExitFromAccount();
        }

        private void DeleteAccountButtonClick(object sender, RoutedEventArgs e)
        {
            MainPage.Client.DeleteMe();
            MainPage.GetInstance().ExitFromAccount();
        }
    }
}