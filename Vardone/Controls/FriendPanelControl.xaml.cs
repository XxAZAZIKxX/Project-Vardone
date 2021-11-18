using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;

namespace Vardone.Controls
{
    /// <summary>
    ///     Interaction logic for FriendPanelControl.xaml
    /// </summary>
    public partial class FriendPanelControl
    {
        private static FriendPanelControl _instance;
        public static FriendPanelControl GetInstance() => _instance ??= new FriendPanelControl();
        public static void ClearInstance() => _instance = null;
        
        public StackPanel FriendList;
        public StackPanel ChatList;

        private FriendPanelControl()
        {
            InitializeComponent();
            FriendList = FriendListGrid;
            ChatList = ChatListGrid;
        }

        private void OpenFriendsProperties(object sender, MouseButtonEventArgs mouseButtonEventArgs) => MainPage.GetInstance().MainFrame.Navigate(FriendsPropertiesPage.GetInstance().Load());
    }
}