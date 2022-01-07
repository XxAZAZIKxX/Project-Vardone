using System;

namespace VardoneEntities.Entities.Guild
{
    public class Member
    {
        public User.User User { get; set; }
        public DateTime JoinDate { get; set; }
        public Guild Guild { get; set; }
        public int NumberInvitedMembers { get; set; }
    }
}