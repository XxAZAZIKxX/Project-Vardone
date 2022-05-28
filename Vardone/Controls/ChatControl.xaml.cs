using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Notification.Wpf;
using Notifications.Wpf;
using Vardone.Controls.Items;
using Vardone.Core;
using Vardone.Pages;
using Vardone.Pages.Popup;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Entities.User;
using VardoneEntities.Models.GeneralModels.Users;
using WpfAnimatedGif;
using Application = System.Windows.Application;
using DataFormats = System.Windows.DataFormats;
using DragDropEffects = System.Windows.DragDropEffects;
using DragEventArgs = System.Windows.DragEventArgs;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace Vardone.Controls
{
    /// <summary>
    /// Логика взаимодействия для ChatControl.xaml
    /// </summary>
    public partial class ChatControl
    {
        private static ChatControl _instance;
        public static ChatControl GetInstance() => _instance ??= new ChatControl();
        public static void ClearInstance() => _instance = null;

        public PrivateChat Chat { get; private set; }
        public Channel Channel { get; private set; }

        public ChatControl()
        {
            InitializeComponent();
            MessageTextBox.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPaste));
        }
        ~ChatControl()
        {
            CloseChat();
            GC.Collect();
        }

        public void DeleteMessageOnChat(ChannelMessage message)
        {
            if (Channel is null) return;
            if (Channel.ChannelId != message.Channel.ChannelId) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                var messageItems = ChatMessagesList.Children.Cast<MessageItem>().ToList();
                var firstOrDefault = messageItems.FirstOrDefault(p => p.ChannelMessage.MessageId == message.MessageId);
                if (firstOrDefault is null) return;
                var indexOf = messageItems.IndexOf(firstOrDefault);
                ChatMessagesList.Children.RemoveAt(indexOf);
            });
        }


        public void DeleteMessageOnChat(PrivateMessage message)
        {
            if (Chat is null) return;
            if (Chat.ChatId != message.Chat.ChatId) return;
            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
           {
               var messageItems = ChatMessagesList.Children.Cast<MessageItem>().ToList();
               var firstOrDefault = messageItems.FirstOrDefault(p => p.PrivateMessage.MessageId == message.MessageId);
               if (firstOrDefault is null) return;
               ChatMessagesList.Children.RemoveAt(messageItems.IndexOf(firstOrDefault));

           }), DispatcherPriority.Send);
        }

        public void AddMessage(ChannelMessage message)
        {
            if (Channel?.ChannelId != message.Channel.ChannelId) return;

            var myId = MainPage.Client.GetMe().UserId;
            var ownerId = MainPage.Client.GetGuilds().FirstOrDefault(p => p.Channels.Any(c => c.ChannelId == message.Channel.ChannelId))
                ?.Owner.User.UserId;
            var deleteMode = message.Author.UserId == myId || ownerId == myId
                ? MessageItem.DeleteMode.CanDelete
                : MessageItem.DeleteMode.CannotDelete;

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
           {
               var firstOrDefault = ChatMessagesList.Children.Cast<MessageItem>()
                   .OrderByDescending(m => m.ChannelMessage.MessageId)
                   .FirstOrDefault(m => m.ChannelMessage.MessageId < message.MessageId);
               var index = ChatMessagesList.Children.IndexOf(firstOrDefault) + 1;
               ChatMessagesList.Children.Insert(index, new MessageItem(message, deleteMode));

           }), DispatcherPriority.Send);
        }

        public void AddMessage(PrivateMessage message)
        {
            if (Chat?.ChatId != message.Chat.ChatId) return;
            var myId = MainPage.Client.GetMe().UserId;
            var deleteMode = message.Author.UserId == myId ? MessageItem.DeleteMode.CanDelete : MessageItem.DeleteMode.CannotDelete;

            Application.Current.Dispatcher.BeginInvoke((Action)(() =>
           {
               var firstOrDefault = ChatMessagesList.Children.Cast<MessageItem>()
                   .OrderByDescending(m => m.PrivateMessage.MessageId)
                   .FirstOrDefault(m => m.PrivateMessage.MessageId < message.MessageId);
               var index = ChatMessagesList.Children.IndexOf(firstOrDefault) + 1;
               ChatMessagesList.Children.Insert(index, new MessageItem(message, deleteMode));

           }), DispatcherPriority.Send);
        }

        public void UpdateUser(User user)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (ChatMessagesList.Children.Count == 0) return;
                var messageItems = ChatMessagesList.Children.Cast<MessageItem>().Where(p => p.Author.UserId == user.UserId);
                foreach (var messageItem in messageItems) messageItem.UpdateUser(user);
            }, DispatcherPriority.Background);
        }

        public void UpdateUserOnline(User user)
        {
            if (Chat is not null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    () => PrivateChatHeader.Children.Cast<UserItem>().FirstOrDefault(p => p.User.UserId == user.UserId)
                        ?.UpdateUserOnline(), DispatcherPriority.Background);
            }

            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                var messageItems = ChatMessagesList.Children.Cast<MessageItem>().Where(p => p.Author.UserId == user.UserId);
                foreach (var messageItem in messageItems) messageItem.UpdateUserOnline();
            }, DispatcherPriority.Background);
        }

        private void OnPaste(object sender, ExecutedRoutedEventArgs e)
        {
            if (Chat is null && Channel is null) return;
            if (System.Windows.Clipboard.ContainsText())
            {
                var text = System.Windows.Clipboard.GetText();
                var caretIndex = MessageTextBox.CaretIndex;
                MessageTextBox.Text = MessageTextBox.Text.Insert(caretIndex, text);
                MessageTextBox.CaretIndex = caretIndex + text.Length;
            }
            if (System.Windows.Clipboard.ContainsImage())
            {
                var image = System.Windows.Clipboard.GetImage();
                if (image is null) return;
                var encoder = new PngBitmapEncoder();
                byte[] bytes;
                using (var stream = new MemoryStream())
                {
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(stream);
                    bytes = stream.ToArray();
                    stream.Close();
                }
                MainPage.GetInstance().MainFrame.Navigate(ImageMessagePreview.GetInstance().Load(ImageWorker.BytesToBitmapImage(bytes), MessageTextBox.Text));
            }
        }
        public void LoadChat(PrivateChat privateChat)
        {
            if (privateChat is null)
            {
                CloseChat();
                return;
            }
            Loading.Visibility = Visibility.Visible;
            ImageBehavior.GetAnimationController(LoadingGif).Play();
            CloseChat();
            Chat = privateChat;
            if (Chat.ChatId == -1) Chat.ChatId = MainPage.Client.GetPrivateChatWithUser(privateChat.ToUser.UserId).ChatId;
            Task.Run(() =>
            {
                var user = MainPage.Client.GetUser(privateChat.ToUser.UserId);
                var me = MainPage.Client.GetMe().UserId;
                var onlineUser = MainPage.Client.GetOnlineUser(user.UserId);
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    var userHeader = new UserItem(user, UserItemType.Profile)
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    userHeader.SetStatus(onlineUser);
                    PrivateChatHeader.Children.Add(userHeader);
                }), DispatcherPriority.Render);

                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    foreach (var message in MainPage.Client.GetPrivateMessagesFromChat(privateChat.ChatId, 15).OrderBy(p => p.MessageId))
                    {
                        var mode = message.Author.UserId == me
                                ? MessageItem.DeleteMode.CanDelete
                                : MessageItem.DeleteMode.CannotDelete;

                        var messageItem = new MessageItem(message, mode);
                        if (messageItem.Author.UserId == user.UserId) messageItem.SetStatus(onlineUser);
                        ChatMessagesList.Children.Add(messageItem);
                        Loading.Visibility = Visibility.Collapsed;
                        ImageBehavior.GetAnimationController(LoadingGif).Pause();
                    }
                }), DispatcherPriority.Send).Task.ContinueWith(_ => Application.Current.Dispatcher.Invoke(() => ChatScrollViewer.ScrollToEnd()));

                var privateChatWithUser = MainPage.Client.GetPrivateChatWithUser(user.UserId);

                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    foreach (var userItem in MainPage.GetInstance().friendListPanel.ChatListGrid.Children.Cast<UserItem>().Where(p => p.User.UserId != user.UserId))
                    {
                        if (userItem.User.UserId != user.UserId) continue;
                        userItem.SetCountMessages(privateChatWithUser.UnreadMessages);
                    }
                }), DispatcherPriority.Background);
            });
        }
        public void LoadChat(Channel openChannel)
        {
            if (openChannel is null)
            {
                CloseChat();
                return;
            }
            Loading.Visibility = Visibility.Visible;
            ImageBehavior.GetAnimationController(LoadingGif).Play();
            CloseChat();
            Channel = openChannel;
            Task.Run(() =>
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() => PrivateChatHeader.Children.Add(new HeaderChannelNameItem(openChannel))), DispatcherPriority.Render);
                var me = MainPage.Client.GetMe().UserId;
                var owner = MainPage.Client.GetGuilds()
                    .FirstOrDefault(p => p.Channels.Any(channel1 => channel1.ChannelId == openChannel.ChannelId))
                    ?.Owner?.User?.UserId;
                var messages = MainPage.Client.GetChannelMessages(openChannel.ChannelId, 15).OrderBy(p => p.MessageId);
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    foreach (var message in messages)
                    {
                        var mode = message.Author.UserId == me || me == owner
                                ? MessageItem.DeleteMode.CanDelete
                                : MessageItem.DeleteMode.CannotDelete;
                        ChatMessagesList.Children.Add(new MessageItem(message, mode));
                    }
                }), DispatcherPriority.Send).Task.ContinueWith(_ => Application.Current.Dispatcher.Invoke(() =>
                {
                    ChatScrollViewer.ScrollToEnd();
                    Loading.Visibility = Visibility.Collapsed;
                    ImageBehavior.GetAnimationController(LoadingGif).Pause();
                }));
            });
        }
        public void CloseChat()
        {
            Chat = null;
            Channel = null;
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                PrivateChatHeader.Children.Clear();
                ChatMessagesList.Children.Clear();
            }, DispatcherPriority.Render);
            GC.Collect();
            MainWindow.FlushMemory();
        }

        private void ChatScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(() =>
            {
                if (ChatScrollViewer.VerticalOffset != 0) return;
                if (Chat is null && Channel is null) return;
                if (ChatMessagesList.Children.Count == 0) return;

                int numberAdded;

                if (Chat is not null)
                {
                    var list = MainPage.Client.GetPrivateMessagesFromChat(Chat.ChatId, 10, ((MessageItem)ChatMessagesList.Children[0]).PrivateMessage.MessageId);
                    numberAdded = list.Length;
                    foreach (var message in list)
                    {
                        var messageItem = new MessageItem(message);
                        messageItem.SetStatus(MainPage.Client.GetOnlineUser(message.Author.UserId));
                        ChatMessagesList.Children.Insert(0, messageItem);
                    }
                }
                else
                {
                    var list = MainPage.Client.GetChannelMessages(Channel.ChannelId, 10, ((MessageItem)ChatMessagesList.Children[0]).ChannelMessage.MessageId);
                    numberAdded = list.Length;
                    foreach (var message in list)
                    {
                        var messageItem = new MessageItem(message);
                        messageItem.SetStatus(MainPage.Client.GetOnlineUser(message.Author.UserId));
                        ChatMessagesList.Children.Insert(0, messageItem);
                    }
                }
                if (numberAdded > 0) ChatScrollViewer.ScrollToVerticalOffset(ChatScrollViewer.ScrollableHeight);
            });
        }
        //Placeholders
        private void MessageBoxGotFocus(object sender, RoutedEventArgs e) =>
            MessageTextBoxPlaceholder.Visibility = Visibility.Collapsed;
        private void MessageBoxLostFocus(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(MessageTextBox.Text)) return;
            MessageTextBoxPlaceholder.Visibility = Visibility.Visible;
        }
        private void MessageTextBoxOnTextChanged(object sender, TextChangedEventArgs e) => MessageBoxGotFocus(null, null);
        private void MessageBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            if (Channel is null && Chat is null) return;
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text)) return;
            SendMessage(MessageTextBox.Text);
            MessageTextBox.Text = "";
            MessageBoxLostFocus(null, null);
        }
        private void ClipMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Chat is null && Channel is null) return;
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Изображение .png|*.png|Изображение .jpg|*.jpg",
                Multiselect = false,
                CheckFileExists = true,
                Title = "Отправить изображение"
            };
            var dialogResult = openFileDialog.ShowDialog();
            if (dialogResult != DialogResult.OK) return;
            if (!openFileDialog.CheckFileExists) return;
            var bitmapImage = ImageWorker.BytesToBitmapImage(File.ReadAllBytes(openFileDialog.FileName));
            if (bitmapImage is null)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Оповещение",
                    Message = "Выберите действительное изображение!",
                    Type = NotificationType.Warning
                });
                return;
            }

            MainPage.GetInstance()
                .MainFrame.Navigate(ImageMessagePreview.GetInstance()
                    .Load(bitmapImage,
                        MessageTextBox.Text));

            MessageTextBox.Clear();
        }
        public void SendMessage(string text, byte[] bytes = null)
        {
            var message = new MessageModel
            {
                Text = text,
            };
            if (bytes != null && ImageWorker.IsImage(bytes)) message.Base64Image = Convert.ToBase64String(bytes);

            if (Chat is not null) MainPage.Client.SendPrivateMessage(Chat.ToUser.UserId, message);
            else if (Channel is not null) MainPage.Client.SendChannelMessage(Channel.ChannelId, message);
        }

        private void TextBoxDragEnter(object sender, DragEventArgs e)
        {
            if (Chat is null && Channel is null) return;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop) && !e.Data.GetDataPresent(DataFormats.Text)) return;

            e.Effects = DragDropEffects.Copy;
            MessageTextBox.Focus();
            e.Handled = true;
        }

        private void TextBoxDragDrop(object sender, DragEventArgs e)
        {
            if (Chat is null && Channel is null) return;
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var file = (e.Data.GetData(DataFormats.FileDrop) as string[])?.FirstOrDefault();
            if (file is null) return;
            var bytes = File.ReadAllBytes(file);

            var bitmapImage = ImageWorker.BytesToBitmapImage(bytes);
            if (bitmapImage is null)
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Title = "Оповещение",
                    Type = NotificationType.Warning,
                    Message = "Данный тип файла не поддерживается!"
                });
                return;
            }
            MainPage.GetInstance().MainFrame.Navigate(ImageMessagePreview.GetInstance().Load(bitmapImage, MessageTextBox.Text));
        }
    }
}