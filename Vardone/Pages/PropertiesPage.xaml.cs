using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using VardoneEntities.Entities;
using Vardone.Core;
using Notifications.Wpf;

namespace Vardone.Pages
{
    /// <summary>
    /// Логика взаимодействия для PropertiesPage.xaml
    /// </summary>
    /// private static MainPage _instance;
    public partial class PropertiesPage : Page
    {
        public User User { get; private set; }
        private static PropertiesPage _instance;
        public static PropertiesPage GetInstance() => _instance ??= new PropertiesPage();
        private PropertiesPage()
        {
            InitializeComponent();
        }
        public void Load(User user)
        {
            User = user;
            AvatarImage.ImageSource = user.Base64Avatar == null
                ? MainPage.DefaultAvatar
                : ImageWorker.BytesToBitmapImage(Convert.FromBase64String(user.Base64Avatar));
            UsernameLabel.Content = user.Username;
            Username_tb.Text = user.Username;
            Email_tb.Text = user.Email;
            Desc_tb.Text = (user.Description == null) ? "Description" : user.Description;

        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) => MainPage.GetInstance().MainFrame.Navigate(null);

        private void Button_SaveClick(object sender, RoutedEventArgs e)
        {
            changeButton.Content = (changeButton.Content.ToString() == "Сохранить") ? "Изменить" : "Сохранить";
            CancelBtn.Visibility = (CancelBtn.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Passwordbox1.Visibility = (Passwordbox1.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Passwordbox2.Visibility = (Passwordbox2.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;

        }
        private void Button_CancelClick(object sender, RoutedEventArgs e)
        {
            changeButton.Content = (changeButton.Content.ToString() == "Сохранить") ? "Изменить" : "Сохранить";
            CancelBtn.Visibility = (CancelBtn.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Passwordbox1.Visibility = (Passwordbox1.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            Passwordbox2.Visibility = (Passwordbox2.Visibility == Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
            //if (string.IsNullOrWhiteSpace(Username_tb.Text))
            //{
            //    MainWindow.GetInstance().notificationManager.Show(new NotificationContent
            //    {
            //        Title = "Проверьте введенные данные",
            //        Message = "Пожалуйста введите пароль корректно",
            //        Type = NotificationType.Warning

            //    }); 
            //}
            //else
            //{
            //    if (Username_changeButton.Content.ToString() == "Сохранить")
            //    {
            //        MainPage._client.UpdateUser(new VardoneEntities.Models.GeneralModels.Users.UpdateUserModel
            //        {
            //            Password = Passwordbox1.Password
            //        });
            //    }
            //}
        } private void Username_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Username_tb.Text))
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Пожалуйста введите логин корректно",
                    Type = NotificationType.Warning

                }); return;
            }
            else
            {
                if (Username_changeButton.Content.ToString() == "Сохранить")
                {
                    MainPage._client.UpdateUser(new VardoneEntities.Models.GeneralModels.Users.UpdateUserModel
                    {
                        Username = Username_tb.Text
                    });
                }
            }
            Username_changeButton.Content = Username_changeButton.Content.ToString() == "Сохранить" ? "Изменить" : "Сохранить";
            Username_tb.IsEnabled = Username_changeButton.Content.ToString() == "Сохранить" ? true : false;

        }

        private void Email_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Email_tb.Text))
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Проверьте введенные данные",
                    Message = "Пожалуйста введите email корректно",
                    Type = NotificationType.Warning

                }); return;
            }
            else
            {
                if (Email_changeButton.Content.ToString() == "Сохранить")
                {
                    MainPage._client.UpdateUser(new VardoneEntities.Models.GeneralModels.Users.UpdateUserModel
                    {
                        Email = Email_tb.Text
                    });
                }
            }
            Email_changeButton.Content = (Email_changeButton.Content.ToString() == "Сохранить") ? "Изменить" : "Сохранить";
            Email_tb.IsEnabled = Email_changeButton.Content.ToString() == "Сохранить" ? true : false;
        }
        private void Description_ChangeBtn(object sender, RoutedEventArgs e)
        {
            if (Desc_changeButton.Content.ToString() == "Сохранить")
            {
                MainPage._client.UpdateUser(new VardoneEntities.Models.GeneralModels.Users.UpdateUserModel
                {
                    Description = Desc_tb.Text
                });
            }
            Desc_changeButton.Content = (Desc_changeButton.Content.ToString() == "Сохранить") ? "Изменить" : "Сохранить";
            Desc_tb.IsEnabled = Desc_changeButton.Content.ToString() == "Сохранить" ? true : false;
        }

        private void Change_Avatar(object sender, MouseButtonEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg",
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            MainPage._client.UpdateUser(new VardoneEntities.Models.GeneralModels.Users.UpdateUserModel
            {
                Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
            });
        }
    }
}
