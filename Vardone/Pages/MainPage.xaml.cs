using System.Windows.Input;
using Vardone.Pages.Profile;
using VardoneLibrary.Core;

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

            /*var i = 0;
            foreach (var friend in user1.GetFriends())
            {
                var friendGridItem = new FriendGridItem(friend) { Margin = new Thickness(0, i, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                FriendsGrid.Children.Add(friendGridItem);
                i = (int)(friendGridItem.Height + friendGridItem.Margin.Top);
            }
            i = 0;
            foreach (var chat in user1.GetPrivateChats())
            {
                var friendGridItem = new FriendGridItem(chat.ToUser) { Margin = new Thickness(0, i, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                MessagesGrid.Children.Add(friendGridItem);
                i = (int)(friendGridItem.Height + friendGridItem.Margin.Top);
            }
            i = 0;
            foreach (var message in user1.GetPrivateMessagesFromChat(user1.GetPrivateChatWithUser(user6.UserId).ChatId))
            {
                var messageItem = new MessageChatItem(message) { Margin = new Thickness(0, i, 0, 0), VerticalAlignment = VerticalAlignment.Top };
                var add = ChatMessagesGrid.Children.Add(messageItem);
                i = (int)(messageItem.HeightItem + messageItem.Margin.Top);
            }*/
            //user1.GetPrivateMessagesFromChat()
        }

        public void Load(VardoneClient client)
        {
            _client = client;
        }
        private void UserProfileOpen(object s, MouseEventArgs e) => FrameUserProfile.Navigate(UserProfile.GetInstance());
    }
}
