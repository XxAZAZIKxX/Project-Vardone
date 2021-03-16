using System;

namespace VardoneEntities.Entities
{
    public class PrivateChat
    {
        public long ChatId { get; init; }
        public User FromUser { get; init; }
        public User ToUser { get; init; }

        public override bool Equals(object? secondChat)
        {
            if (secondChat is null) return false;
            return secondChat is PrivateChat chat && Equals(chat);
        }

        protected bool Equals(PrivateChat other) => ChatId == other.ChatId && FromUser.Equals(other.FromUser) && ToUser.Equals(other.ToUser);
        public override int GetHashCode() => HashCode.Combine(ChatId, FromUser, ToUser);
        public static bool operator ==(PrivateChat left, PrivateChat right) => left is not null && left.Equals(right);
        public static bool operator !=(PrivateChat left, PrivateChat right) => left is not null && !left.Equals(right);
    }
}