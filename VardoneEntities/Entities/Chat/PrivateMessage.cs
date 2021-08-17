using System;

namespace VardoneEntities.Entities
{
    public class PrivateMessage
    {
        public long MessageId { get; init; }
        public PrivateChat Chat { get; init; }
        public User Author { get; init; }
        public DateTime CreateTime { get; init; }
        public string Text { get; init; }
        public string Base64Image { get; init; }

#nullable enable
        public override bool Equals(object? secondMessage)
        {
            if (secondMessage is null) return false;
            return secondMessage is PrivateMessage message && Equals(message);
        }

        private bool Equals(PrivateMessage other) => MessageId == other.MessageId && Chat.Equals(other.Chat) && Author.Equals(other.Author) && CreateTime.Equals(other.CreateTime) && Text == other.Text && Base64Image == other.Base64Image;
        public override int GetHashCode() => HashCode.Combine(MessageId, Chat, Author, CreateTime, Text, Base64Image);
        public static bool operator ==(PrivateMessage left, PrivateMessage right) => left.Equals(right);
        public static bool operator !=(PrivateMessage left, PrivateMessage right) => !left.Equals(right);
    }
}