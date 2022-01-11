using System;

namespace VardoneEntities.Entities.Guild
{
    public record GuildInvite
    {
        public long InviteId { get; init; } = -1;
        public Guild Guild { get; init; }
        public User.User CreatedBy { get; init; }
        public DateTime CreatedAt { get; init; }
        public int NumberOfUses { get; init; }
        public string InviteCode { get; init; }
    }
}