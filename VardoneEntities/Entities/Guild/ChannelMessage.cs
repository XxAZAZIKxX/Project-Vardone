using System;

namespace VardoneEntities.Entities.Guild
{
    public record ChannelMessage
    {
        public long MessageId { get; set; } = -1;
        public Channel Channel { get; set; }
        public User.User Author { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Text { get; set; }
        public string Base64Image { get; set; }
    }
}