using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Input;
using Vardone.Controls.ItemControls;
using Vardone.Core;
using Vardone.Pages.Profile;
using VardoneEntities.Entities;
using VardoneEntities.Models.GeneralModels.PrivateChats;
using VardoneEntities.Models.GeneralModels.Users;
using VardoneLibrary.Core;
using VardoneLibrary.Core.Base;

namespace Vardone.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage
    {
        private static MainPage _instance;
        public static MainPage GetInstance() => _instance ??= new MainPage();
        private static VardoneClient _client;
        private MainPage()
        {
            InitializeComponent();
            try
            {
                VardoneBaseApi.RegisterUser(new RegisterUserModel { Username = "Julie", Email = "test@", Password = "1" });
                VardoneBaseApi.RegisterUser(new RegisterUserModel { Username = "Ahri", Email = "test1@", Password = "1" });
                VardoneBaseApi.RegisterUser(new RegisterUserModel { Username = "Katarina", Email = "test2@", Password = "1" });
            }
            catch
            {
                // ignored
            }

            var token1 = VardoneBaseApi.GetUserToken("test@", "1");
            var token2 = VardoneBaseApi.GetUserToken("test1@", "1");
            var token3 = VardoneBaseApi.GetUserToken("test2@", "1");
            
            var julie = new VardoneClient(token1.UserId, token1.Token);
            var ahri = new VardoneClient(token2.UserId, token2.Token);
            var kat = new VardoneClient(token3.UserId, token3.Token);

            //julie.UpdateUser(new UpdateUserModel
            //{
            //    Base64Image = Convert.ToBase64String(File.ReadAllBytes(@"D:\User\Downloads\photo_2021-03-10_20-42-00.jpg"))
            //});
            //ahri.UpdateUser(new UpdateUserModel
            //{
            //    Base64Image = Convert.ToBase64String(File.ReadAllBytes(@"D:\User\Pictures\Pictures\4501618-anime-anime-girls-picture-in-picture.png"))
            //});
            //kat.UpdateUser(new UpdateUserModel
            //{
            //    Base64Image = Convert.ToBase64String(File.ReadAllBytes(@"D:\User\Pictures\Pictures\a77fdf812e91550b07452788b7dacbd4.png"))
            //});

            julie.AddFriend(4);
            ahri.AddFriend(4);
            kat.AddFriend(4);
            //ahri.SendPrivateMessage(4, new PrivateMessageModel{Text = "Hi", Base64Image = Convert.ToBase64String(File.ReadAllBytes(@"D:\User\Downloads\photo_2021-03-11_11-13-46.jpg"))});
        }

        public void Load(VardoneClient client)
        {
            _client = client;
            var i = 0;
            foreach (var friend in _client.GetFriends())
            {
                var friendGridItem = new FriendGridItem(friend) { Margin = new Thickness(0, i, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                FriendsGrid.Children.Add(friendGridItem);
                i = (int)(friendGridItem.Height + friendGridItem.Margin.Top);
            }
            i = 0;
            foreach (var chat in _client.GetPrivateChats())
            {
                var friendGridItem = new FriendGridItem(chat.ToUser) { Margin = new Thickness(0, i, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                MessagesGrid.Children.Add(friendGridItem);
                i = (int)(friendGridItem.Height + friendGridItem.Margin.Top);
            }
            //client.UpdateUser(new UpdateUserModel
            //{
            //    Username = @"r\\Kzenta",
            //    //Base64Image = Convert.ToBase64String(File.ReadAllBytes(@"D:\User\Pictures\152403578_223907286105845_277220794561786358_n.jpg"))
            //});

            var me = _client.GetMe();
            MyUsername.Text = me.Username;
            MyAvatar.ImageSource = me.Base64Avatar == null ? null : Base64ToBitmap.ToImage(Convert.FromBase64String(me.Base64Avatar));

        }

        public void LoadPrivateChat(long userId)
        {
            var i = 0;
            ChatMessagesGrid.Children.Clear();
            foreach (var message in _client.GetPrivateMessagesFromChat(_client.GetPrivateChatWithUser(userId).ChatId))
            {
                var messageItem = new MessageChatItem(message) { Margin = new Thickness(0, i, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                ChatMessagesGrid.Children.Add(messageItem);
                i = (int)(messageItem.HeightItem + messageItem.Margin.Top);
            }
        }
        private void UserProfileOpen(object s, MouseEventArgs e) => FrameUserProfile.Navigate(UserProfile.GetInstance());
    }
}
