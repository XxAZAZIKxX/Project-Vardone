using System;

namespace VardoneEntities.Entities
{
    public class User
    {
        public long UserId { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Base64Avatar { get; init; }
        public string Description { get; init; }

        public override bool Equals(object? secondUser)
        {
            if (secondUser is null) return false;
            return secondUser is User user && Equals(user);
        }

        protected bool Equals(User other) => UserId == other.UserId && Username == other.Username && Email == other.Email && Base64Avatar == other.Base64Avatar && Description == other.Description;

        public override int GetHashCode() => HashCode.Combine(UserId, Username, Email, Base64Avatar, Description);

        public static bool operator ==(User left, User right) => left is not null && left.Equals(right);

        public static bool operator !=(User left, User right) => left is not null && !left.Equals(right);
    }
}