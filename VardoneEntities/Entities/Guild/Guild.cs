using System;
using System.Collections.Generic;

namespace VardoneEntities.Entities.Guild
{
    public class Guild
    {
        public long GuildId { get; init; } = -1;
        public string Name { get; set; }
        public string Base64Avatar { get; set; }
        public User Owner { get; set; }
        public List<Channel> Channels { get; set; }
        
        public override bool Equals(object obj) => obj is Guild guild && Equals(guild);
        private bool Equals(Guild other) => GuildId == other.GuildId && Name == other.Name && Base64Avatar == other.Base64Avatar;
        public override int GetHashCode() => HashCode.Combine(GuildId, Name, Base64Avatar);

        public static bool operator ==(Guild left, Guild right) => left is not null && left.Equals(right);
        public static bool operator !=(Guild left, Guild right) => !(left == right);
    }
}