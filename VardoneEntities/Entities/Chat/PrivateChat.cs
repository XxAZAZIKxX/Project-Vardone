using System;

namespace VardoneEntities.Entities.Chat
{
    /// <summary>
    /// Представляет объект приватного чата
    /// </summary>
    public class PrivateChat
    {
        public long ChatId { get; set; } = -1;
        public User FromUser { get; set; }
        public User ToUser { get; set; }
        public int UnreadMessages { get; set; }

#nullable enable
        public override bool Equals(object? secondChat)
        {
            if (secondChat is null) return false;
            return secondChat is PrivateChat chat && Equals(chat);
        }

        private bool Equals(PrivateChat other) => ChatId == other.ChatId && FromUser.Equals(other.FromUser) && ToUser.Equals(other.ToUser) && UnreadMessages == other.UnreadMessages;

        public override int GetHashCode() => HashCode.Combine(ChatId, FromUser, ToUser, UnreadMessages);

        public static bool operator ==(PrivateChat left, PrivateChat right) => Equals(left, right);

        public static bool operator !=(PrivateChat left, PrivateChat right) => !Equals(left, right);
    }
}