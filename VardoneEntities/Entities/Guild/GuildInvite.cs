using System;

namespace VardoneEntities.Entities.Guild
{
    public class GuildInvite
    {
        public long InviteId { get; set; } = -1;
        public Guild Guild { get; set; }
        public User.User CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public int NumberOfUses { get; set; }
        public string InviteCode { get; set; }
    }
}