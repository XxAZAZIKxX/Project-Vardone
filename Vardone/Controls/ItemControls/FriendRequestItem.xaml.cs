using System;
using System.Windows.Controls;
using System.Windows.Input;
using Vardone.Core;
using Vardone.Pages;
using VardoneEntities.Entities;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for FriendRequestItem.xaml
    /// </summary>
    public partial class FriendRequestItem : UserControl
    {
        public User user;
        public FriendRequestItem(User user)
        {
            InitializeComponent();
     
        }
    }
}
