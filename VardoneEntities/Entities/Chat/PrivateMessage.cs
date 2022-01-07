using System;

namespace VardoneEntities.Entities.Chat
{
    public class PrivateMessage
    {
        public long MessageId { get; set; } = -1;
        public PrivateChat Chat { get; set; }
        public User.User Author { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Text { get; set; }
        public string Base64Image { get; set; }

#nullable enable
        public override bool Equals(object? secondMessage)
        {
            if (secondMessage is null) return false;
            return secondMessage is PrivateMessage message && Equals(message);
        }

        private bool Equals(PrivateMessage other) => MessageId == other.MessageId && Chat.Equals(other.Chat) && Author.Equals(other.Author) && CreatedTime.Equals(other.CreatedTime) && Text == other.Text && Base64Image == other.Base64Image;
        public override int GetHashCode() => HashCode.Combine(MessageId, Chat, Author, CreatedTime, Text, Base64Image);
        public static bool operator ==(PrivateMessage left, PrivateMessage right) => left.Equals(right);
        public static bool operator !=(PrivateMessage left, PrivateMessage right) => !left.Equals(right);
    }
}