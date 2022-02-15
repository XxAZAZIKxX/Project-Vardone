using System;

namespace VardoneEntities.Entities.Guild
{
    public record Member
    {
        public User.User User { get; set; }
        public DateTime JoinDate { get; init; }
        public Guild Guild { get; init; }
        public int NumberInvitedMembers { get; set; }
    }
}