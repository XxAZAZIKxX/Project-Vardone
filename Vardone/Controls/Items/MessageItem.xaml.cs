﻿using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Логика взаимодействия для MessageGridItem.xaml
    /// </summary>
    public partial class MessageItem
    {
        public PrivateMessage PrivateMessage { get; }
        public ChannelMessage ChannelMessage { get; }
        public User Author { get; }

        public enum DeleteMode
        {
            CanDelete, CannotDelete
        }

        public MessageItem(PrivateMessage message, DeleteMode mode = DeleteMode.CannotDelete)
        {
            InitializeComponent();
            SetDeleteButton(mode);

            PrivateMessage = message;
            Author = message.Author;

            SetStatus(MainPage.Client.GetOnlineUser(message.Author.UserId));

            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(Author.UserId);

            CreatedTime.Content = message.CreatedTime.ToShortDateString() + " " + message.CreatedTime.ToShortTimeString();
            Username.Content = Author.Username;
            Text.Text = message.Text;
            if (message.Base64Image is not null) Image.Source = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(message.Base64Image));
        }

        public MessageItem(ChannelMessage channelMessage, DeleteMode mode = DeleteMode.CannotDelete)
        {
            InitializeComponent();

            SetDeleteButton(mode);
            ChannelMessage = channelMessage;
            Author = channelMessage.Author;

            SetStatus(MainPage.Client.GetOnlineUser(channelMessage.Author.UserId));

            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(Author.UserId);

            CreatedTime.Content = channelMessage.CreatedTime.ToShortDateString() + " " + channelMessage.CreatedTime.ToShortTimeString();
            Username.Content = Author.Username;
            Text.Text = channelMessage.Text;
            if (channelMessage.Base64Image is not null) Image.Source = ImageWorker.BytesToBitmapImage(Convert.FromBase64String(channelMessage.Base64Image));
        }

        public void UpdateUserOnline() => SetStatus(MainPage.Client.GetOnlineUser(Author.UserId));

        public void UpdateUser(User user)
        {
            if (user is null) return;
            Username.Content = user.Username;
            Avatar.ImageSource = AvatarsWorker.GetAvatarUser(user.UserId);
        }

        private void SetDeleteButton(DeleteMode mode)
        {
            if (mode is DeleteMode.CannotDelete)
            {
                ContextMenu.IsEnabled = false;
                ContextMenu.Visibility = Visibility.Collapsed;
            }
        }

        private void ImageOnClick(object sender, MouseButtonEventArgs e) => MainPage.GetInstance().DeployImage(Image.Source as BitmapImage);

        public void SetStatus(bool online)
        {
            OnlineStatus.Fill = online switch
            {
                true => new SolidColorBrush(Colors.LimeGreen),
                false => new SolidColorBrush(Color.FromRgb(80, 80, 80))
            };
        }

        private void DeleteMessageButtonClicked(object sender, RoutedEventArgs e)
        {
            if (ChannelMessage is not null) MainPage.Client.DeleteChannelMessage(ChannelMessage.MessageId);
            if (PrivateMessage is not null) MainPage.Client.DeletePrivateMessage(PrivateMessage.MessageId);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            string message = Text.Text; 
            String word = sender.ToString().Replace(" ","").Substring(39,(sender.ToString().Length-54));
            if (MessageBox.Show($"Вы уверенны, что хотите пожаловаться на {word}?","Подтверждение"
                , MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                MessageBox.Show("fine");
                //Саша ворд хранит жалобу, можешь пихать ее куда хочешь. 
                //Можешь даже заменить этим словом ник автора сообщения на неск дней.
            }

        }
    }
}
