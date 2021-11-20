﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Notifications.Wpf;
using Vardone.Controls.Items;

namespace Vardone.Pages.PropertyPages
{
    /// <summary>
    /// Interaction logic for FriendsPropertiesPage.xaml
    /// </summary>
    public partial class FriendsPropertiesPage
    {
        private static FriendsPropertiesPage _instance;
        public static FriendsPropertiesPage GetInstance() => _instance ??= new FriendsPropertiesPage();
        public static void ClearInstance() => _instance = null;
        private FriendsPropertiesPage() => InitializeComponent();
        //Loads
        public FriendsPropertiesPage Load()
        {
            LoadIncomingRequests();
            LoadOutgoingRequests();
            return this;
        }

        public void LoadIncomingRequests()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                IncomingRequest.Children.Clear();
                var requests = MainPage.Client.GetIncomingFriendRequests();
                foreach (var friendRequestItem in requests.OrderBy(p => p.Username).Select(user => new FriendRequestItem(user, RequestType.Incoming)))
                    IncomingRequest.Children.Add(friendRequestItem);
            });
        }

        public void LoadOutgoingRequests()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                OutgoingRequest.Children.Clear();
                var requests = MainPage.Client.GetOutgoingFriendRequests();
                foreach (var friendRequestItem in requests.OrderBy(p => p.Username)
                    .Select(user => new FriendRequestItem(user, RequestType.Outgoing)))
                    OutgoingRequest.Children.Add(friendRequestItem);
            });
        }

        private void CloseMouseDown(object sender, MouseButtonEventArgs e) =>
            MainPage.GetInstance().MainFrame.Navigate(null);

        private void AddFriendClick(object sender, RoutedEventArgs e)
        {
            if (@TbFriendName.Text.Trim() == MainPage.Client.GetMe().Username)
            {
                MainWindow.GetInstance()
                    .notificationManager.Show(new NotificationContent
                    {
                        Title = "Некорректное имя пользователя",
                        Message = "Нельзя добавить себя в друзья",
                        Type = NotificationType.Error
                    });
                return;
            }

            try
            {
                MainPage.Client.AddFriend(@TbFriendName.Text.Trim());
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Успех",
                    Message = "Заявка была успешно отправлена",
                    Type = NotificationType.Success
                });
                TbFriendName.Text = "";
                LoadOutgoingRequests();
                LoadIncomingRequests();
            }
            catch
            {
                MainWindow.GetInstance()
                    .notificationManager.Show(new NotificationContent
                    {
                        Title = "Некорректное имя пользователя",
                        Message = "Такого пользователя не существует",
                        Type = NotificationType.Error
                    });
            }
        }
    }
}