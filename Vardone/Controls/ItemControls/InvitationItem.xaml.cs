using VardoneEntities.Entities.Guild;

namespace Vardone.Controls.ItemControls
{
    /// <summary>
    /// Interaction logic for InvitationItem.xaml
    /// </summary>
    public partial class InvitationItem
    {
        public GuildInvite invite;
        public InvitationItem(GuildInvite invite)
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
