using System;

namespace VardoneEntities.Entities.Chat
{
    public record PrivateMessage
    {
        public long MessageId { get; init; } = -1;
        public PrivateChat Chat { get; set; }
        public User.User Author { get; init; }
        public DateTime CreatedTime { get; init; }
        public string Text { get; set; }
        public string Base64Image { get; set; }
    }
}