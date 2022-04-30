using System;
using System.Windows;
using Notification.Wpf;
using Notifications.Wpf;
using Vardone.Pages;
using Vardone.Pages.PropertyPages;
using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.Items
{
    /// <summary>
    /// Interaction logic for GuildInvitationItem.xaml
    /// </summary>
    public partial class GuildInvitationItem
    {
        public GuildInvite invite;
        public GuildInvitationItem(GuildInvite invite)
        {
            InitializeComponent();
            this.invite = invite;
            User.Child = new UserItem(invite.CreatedBy, UserItemType.View)
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            InviteCode.Content = invite.InviteCode;
            NumberOfUses.Content = invite.NumberOfUses;
            var expiresInContent = invite.CreatedAt.AddDays(1);
            ExpiresIn.Content = $"{expiresInContent.ToShortDateString()} {expiresInContent.ToLongTimeString()}";
        }

        private void DeleteInvintationButton(object sender, RoutedEventArgs e)
        {
            try
            {
                MainPage.Client.DeleteGuildInvite(invite.InviteId);
            }
            catch
            {
                MainWindow.GetInstance().notificationManager.Show(new NotificationContent
                {
                    Type = NotificationType.Error,
                    Title = "Ошибка",
                    Message = "Что-то пошло не так"
                });
            }
        }
    }
}
