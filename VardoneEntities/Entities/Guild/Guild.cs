using System.Collections.Generic;

namespace VardoneEntities.Entities.Guild
{
    public record Guild
    {
        public long GuildId { get; set; } = -1;
        public string Name { get; set; }
        public string Base64Avatar { get; set; }
        public Member Owner { get; set; }
        public Channel[] Channels { get; set; }
    }
}