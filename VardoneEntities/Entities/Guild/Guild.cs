using System;

namespace VardoneEntities.Entities
{
    public class Guild
    {
        public long GuildId { get; init; }
        public string Name { get; init; }
        public string Base64Avatar { get; init; }
        
        public override bool Equals(object obj) => obj is Guild guild && Equals(guild);
        private bool Equals(Guild other) => GuildId == other.GuildId && Name == other.Name && Base64Avatar == other.Base64Avatar;
        public override int GetHashCode() => HashCode.Combine(GuildId, Name, Base64Avatar);
        public static bool operator ==(Guild left, Guild right) => left is not null && left.Equals(right);
        public static bool operator !=(Guild left, Guild right) => !(left == right);
    }
}