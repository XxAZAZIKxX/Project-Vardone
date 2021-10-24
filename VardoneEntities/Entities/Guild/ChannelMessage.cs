using System;

namespace VardoneEntities.Entities.Guild
{
    public class ChannelMessage
    {
        public long MessageId { get; set; } = -1;
        public Channel Channel { get; set; }
        public User Author { get; set; }
        public DateTime CreatedTime { get; set; }
        public string Text { get; set; }
        public string Base64Image { get; set; }

        public override bool Equals(object obj) => obj is ChannelMessage message && Equals(message);
        private bool Equals(ChannelMessage other) => MessageId == other.MessageId && Equals(Channel, other.Channel) && Equals(Author, other.Author) && CreatedTime.Equals(other.CreatedTime) && Text == other.Text && Base64Image == other.Base64Image;
        public override int GetHashCode() => HashCode.Combine(MessageId, Channel, Author, CreatedTime, Text, Base64Image);
        public static bool operator ==(ChannelMessage left, ChannelMessage right) => left is not null && left.Equals(right);
        public static bool operator !=(ChannelMessage left, ChannelMessage right) => !(left == right);
    }
}