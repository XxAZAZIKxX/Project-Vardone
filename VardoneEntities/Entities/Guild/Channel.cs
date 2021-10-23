using System;

namespace VardoneEntities.Entities.Guild
{
    public class Channel
    {
        public long ChannelId { get; init; }
        public string Name { get; init; }
        public Guild Guild { get; init; }

        public override bool Equals(object obj) => obj is Channel channel && Equals(channel);
        private bool Equals(Channel other) => ChannelId == other.ChannelId && Name == other.Name && Equals(Guild, other.Guild);
        public override int GetHashCode() => HashCode.Combine(ChannelId, Name, Guild);
        public static bool operator ==(Channel left, Channel right) => left is not null && left.Equals(right);
        public static bool operator !=(Channel left, Channel right) => !(left == right);
    }
}