using System;

namespace VardoneEntities.Entities
{
    public class ChannelMessage
    {
        public long MessageId { get; init; }
        public Channel Channel { get; init; }
        public User Author { get; init; }
        public DateTime CreatedTime { get; init; }
        public string Text { get; init; }
        public string Base64Image { get; init; }

        public override bool Equals(object obj) => obj is ChannelMessage message && Equals(message);
        private bool Equals(ChannelMessage other) => MessageId == other.MessageId && Equals(Channel, other.Channel) && Equals(Author, other.Author) && CreatedTime.Equals(other.CreatedTime) && Text == other.Text && Base64Image == other.Base64Image;
        public override int GetHashCode() => HashCode.Combine(MessageId, Channel, Author, CreatedTime, Text, Base64Image);
        public static bool operator ==(ChannelMessage left, ChannelMessage right) => left is not null && left.Equals(right);
        public static bool operator !=(ChannelMessage left, ChannelMessage right) => !(left == right);
    }
}