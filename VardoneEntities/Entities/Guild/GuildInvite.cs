using System;

namespace VardoneEntities.Entities.Guild
{
    public class GuildInvite
    {
        public long InviteId { get; init; }
        public Guild Guild { get; init; }
        public User CreatedBy { get; init; }
        public DateTime CreatedAt { get; init; }
        public string InviteCode { get; init; }
    }
}