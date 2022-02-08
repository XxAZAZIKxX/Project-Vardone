using System;

namespace VardoneEntities.Models.GeneralModels.Users
{
    public record UserTokenModel
    {
        public long UserId { get; set; }
        public string Token { get; set; }

        public virtual bool Equals(UserTokenModel other) => UserId == other?.UserId && Token == other.Token;

        public override int GetHashCode() => HashCode.Combine(UserId, Token);
    }
}