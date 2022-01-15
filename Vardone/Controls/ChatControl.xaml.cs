using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using Vardone.Controls.Items;
using Vardone.Pages;
using VardoneEntities.Entities.Chat;
using VardoneEntities.Entities.Guild;
using VardoneEntities.Models.GeneralModels.Users;
using Application = System.Windows.Application;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

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
        public ChatControl() => InitializeComponent();

        ~ChatControl()
        {
            CloseChat();
            GC.Collect();
        }

        public PrivateChat chat;
        public Channel channel;

        public void LoadChat(PrivateChat privateChat)
        {
            if (privateChat is null)
            {
                CloseChat();
                return;
            }

            CloseChat();
            this.chat = privateChat;
            if (this.chat.ChatId == -1) this.chat.ChatId = MainPage.Client.GetPrivateChatWithUser(privateChat.ToUser.UserId).ChatId;
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var user = MainPage.Client.GetUser(privateChat.ToUser.UserId);
                    var me = MainPage.Client.GetMe().UserId;
                    var userHeader = new UserItem(user, UserItemType.Profile)
                    {
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    var onlineUser = MainPage.Client.GetOnlineUser(user.UserId);
                    userHeader.SetStatus(onlineUser);
                    PrivateChatHeader.Children.Add(userHeader);
                    foreach (var message in MainPage.Client.GetPrivateMessagesFromChat(privateChat.ChatId, 15).OrderBy(p => p.MessageId))
                    {
                        var mode = message.Author.UserId == me
                            ? MessageItem.DeleteMode.CanDelete
                            : MessageItem.DeleteMode.CannotDelete;
                        var messageItem = new MessageItem(message, mode);
                        if (messageItem.Author.UserId == user.UserId) messageItem.SetStatus(onlineUser);
                        ChatMessagesList.Children.Add(messageItem);
                    }

                    var privateChatWithUser = MainPage.Client.GetPrivateChatWithUser(user.UserId);
                    foreach (var userItem in MainPage.GetInstance().friendListPanel.ChatListGrid.Children.Cast<UserItem>())
                    {
                        if (userItem.User.UserId != user.UserId) continue;
                        userItem.SetCountMessages(privateChatWithUser.UnreadMessages);
                        break;
                    }

                    ChatScrollViewer.ScrollToEnd();
                });
            });
        }
        public void LoadChat(Channel openChannel)
        {
            if (openChannel is null)
            {
                CloseChat();
                return;
            }

            CloseChat();
            this.channel = openChannel;
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PrivateChatHeader.Children.Add(new HeaderChannelNameItem(openChannel));
                    var me = MainPage.Client.GetMe().UserId;
                    var owner = MainPage.Client.GetGuilds()
                        .FirstOrDefault(p => p.Channels.Any(p => p.ChannelId == openChannel.ChannelId))?.Owner?.User?.UserId;
                    var messages = MainPage.Client.GetChannelMessages(openChannel.ChannelId, 15).OrderBy(p => p.MessageId);
                    foreach (var message in messages)
                    {
                        var mode = message.Author.UserId == me || me == owner
                            ? MessageItem.DeleteMode.CanDelete
                            : MessageItem.DeleteMode.CannotDelete;
                        ChatMessagesList.Children.Add(new MessageItem(message, mode));
                    }

                    ChatScrollViewer.ScrollToEnd();

                });
            });
        }
        public void CloseChat()
        {
            chat = null;
            channel = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                PrivateChatHeader.Children.Clear();
                ChatMessagesList.Children.Clear();
            });
            GC.Collect();
            MainWindow.FlushMemory();
        }

        public void UpdateMessages()
        {
            if (chat is not null) LoadChat(chat);
            if (channel is not null) LoadChat(channel);
        }

        private void ChatScrollViewer_OnScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            Task.Run(() =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (ChatScrollViewer.VerticalOffset != 0) return;
                    if (chat is null && channel is null) return;
                    if (ChatMessagesList.Children.Count == 0) return;

                    int numberAdded;

                    if (chat is not null)
                    {
                        var list = MainPage.Client.GetPrivateMessagesFromChat(chat.ChatId, 10, ((MessageItem)ChatMessagesList.Children[0]).PrivateMessage.MessageId);
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
                        var list = MainPage.Client.GetChannelMessages(channel.ChannelId, 10, ((MessageItem)ChatMessagesList.Children[0]).ChannelMessage.MessageId);
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
            if (channel is null && chat is null) return;
            if (string.IsNullOrWhiteSpace(MessageTextBox.Text)) return;
            if (chat is not null)
            {
                MainPage.Client.SendPrivateMessage(chat.ToUser.UserId, new MessageModel { Text = MessageTextBox.Text });
                LoadChat(chat);
            }
            else
            {
                MainPage.Client.SendChannelMessage(channel.ChannelId, new MessageModel { Text = MessageTextBox.Text });
                LoadChat(channel);
            }

            MessageTextBox.Text = "";
            MessageBoxLostFocus(null, null);
        }
        private void ClipMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (chat is null && channel is null) return;
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
            if (chat is not null)
            {
                MainPage.Client.SendPrivateMessage(chat.ToUser.UserId,
                    new MessageModel
                    {
                        Text = MessageTextBox.Text,
                        Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
                    });
                LoadChat(chat);
            }
            else
            {
                MainPage.Client.SendChannelMessage(channel.ChannelId,
                    new MessageModel
                    {
                        Text = MessageTextBox.Text,
                        Base64Image = Convert.ToBase64String(File.ReadAllBytes(openFileDialog.FileName))
                    });
                LoadChat(channel);
            }

            MessageTextBox.Clear();
        }
    }
}