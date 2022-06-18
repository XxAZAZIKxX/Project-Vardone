using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Notification.Wpf;
using Vardone.Controls.Items;
using Vardone.Core;
using VardoneEntities.Models.GeneralModels.Users;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Логика взаимодействия для UserPropertiesPage.xaml
    /// </summary>
    public partial class UserPropertiesPage
    {
        private static UserPropertiesPage _instance;
        public static UserPropertiesPage GetInstance() => _instance ??= new UserPropertiesPage();
        private UserPropertiesPage() => InitializeComponent();
        public static void ClearInstance() => _instance = null;

        public UserPropertiesPage Load()
        {
            var user = MainPage.Client.GetMe();
            Application.Current.Dispatcher.Invoke(() =>
            {
                AvatarImage.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
                UsernameLabel.Content = user.Username;
                UsernameTb.Text = user.Username;
                EmailTb.Text = user.AdditionalInformation.Email;
                DescTb.Text = user.Description ?? "Описание";
                StartPreferencesSp.Children.Clear();
                StartPreferencesSp.Children.Add(new CheckBoxItem("Автозапуск", ConfigWorker.GetAutostart(), ConfigWorker.SetAutostart));
                StartPreferencesSp.Children.Add(new CheckBoxItem("Запускать свернутым", ConfigWorker.GetStartMinimized(), ConfigWorker.SetStartMinimized));
            });
            return this;
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
                    var previousPasswordHash = new StringBuilder();
                    var newPasswordHash = new StringBuilder();
                    using (var sha512 = SHA512.Create())
                    {
                        var computeHash = sha512.ComputeHash(Encoding.ASCII.GetBytes(PasswordBox1.Password));
                        foreach (var b in computeHash) previousPasswordHash.Append(b.ToString("X"));
                        computeHash = sha512.ComputeHash(Encoding.ASCII.GetBytes(PasswordBox2.Password));
                        foreach (var b in computeHash) newPasswordHash.Append(b.ToString("X"));
                    }
                    MainPage.Client.UpdatePassword(new UpdatePasswordModel
                    {
                        PreviousPasswordHash = previousPasswordHash.ToString(),
                        NewPasswordHash = newPasswordHash.ToString()
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

                try
                {
                    MainPage.Client.UpdateMe(new UpdateUserModel
                    {
                        Username = UsernameTb.Text
                    });
                }
                catch
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Message = "Имя пользователя уже занято!",
                        Title = "Ошибка",
                        Type = NotificationType.Error
                    });
                }
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

                try
                {
                    MainPage.Client.UpdateMe(new UpdateUserModel
                    {
                        Email = EmailTb.Text
                    });
                }
                catch
                {
                    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                    {
                        Message = "Почта уже занята!",
                        Title = "Ошибка",
                        Type = NotificationType.Error
                    });
                }

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
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg",
                Multiselect = false,
                Title = "Сменить аватар"
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            MainPage.Client.UpdateMe(new UpdateUserModel
            {
                Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
            });
            AvatarsWorker.UpdateAvatarUser(MainPage.Client.GetMe().UserId);
            Load();
        }

        private void ExitButtonClick(object sender, RoutedEventArgs e)
        {
            MainPage.Client.CloseCurrentSession();
            MainPage.GetInstance().ExitFromAccount();
        }

        private void ExitEverywhereButtonClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(
                "Вы точно хотите выйти со всех устройств? Текущая сессия будет завершена",
                "Подтвердите действие",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            MainPage.Client.CloseAllSessions();
            MainPage.GetInstance().ExitFromAccount();
        }

        private void DeleteAccountButtonClick(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = MessageBox.Show(
                "Вы точно хотите удалить свой аккаунт? Все данные связанные с вами буду удалены. Сервера владелец которых вы являетесь и ваши сообщения будут безвозвратно удалены",
                "Подтвердите действие", MessageBoxButton.YesNo, MessageBoxImage.Stop);
            if (messageBoxResult != MessageBoxResult.Yes) return;
            MainPage.Client.DeleteMe();
            MainPage.GetInstance().ExitFromAccount();
        }

        private void Name_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (NameChangeButton.Content is "Сохранить")
            {
                if (string.IsNullOrWhiteSpace(NameTb.Text))
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
                    FullName = NameTb.Text
                });

                Load();
            }

            NameChangeButton.Content = NameChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            NameTb.IsEnabled = NameChangeButton.Content.ToString() == "Сохранить";
        }
        private void Phone_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (PhoneChangeButton.Content.ToString() == "Сохранить")
            {
                MainPage.Client.UpdateMe(new UpdateUserModel
                {
                    Phone = PhoneTb.Text.Trim()
                });
                Load();
            }

            PhoneChangeButton.Content = PhoneChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            PhoneTb.IsEnabled = PhoneChangeButton.Content.ToString() == "Сохранить";
        }
        private void Post_ChangeBtn(object sender, RoutedEventArgs e)
        {
            //if (PostChangeButton.Content.ToString() == "Сохранить")
            //{
            //    MainPage.Client.UpdateMe(new UpdateUserModel
            //    {
            //        Post = PostTb.Text
            //    });
            //    Load();
            //}

            PostChangeButton.Content = PostChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            //   PostTb.IsEnabled = PostChangeButton.Content.ToString() == "Сохранить";
        }
        private void Birthday_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (BirthdayChangeButton.Content.ToString() == "Сохранить")
            {
                MainPage.Client.UpdateMe(new UpdateUserModel
                {
                    BirthDate = BirthdayTb.SelectedDate
                });
                Load();
            }

            BirthdayChangeButton.Content = BirthdayChangeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            BirthdayTb.IsEnabled = BirthdayChangeButton.Content.ToString() == "Сохранить";
        }
    }
}