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
            User.Child = new UserItem(invite.CreatedBy, UserItemType.View);
            InviteCode.Content = invite.InviteCode;
            NumberOfUses.Content = invite.NumberOfUses;
            var expiresInContent = invite.CreatedAt.AddDays(1);
            ExpiresIn.Content = $"{expiresInContent.ToShortDateString()} {expiresInContent.ToLongTimeString()}";
        }
    }
}
