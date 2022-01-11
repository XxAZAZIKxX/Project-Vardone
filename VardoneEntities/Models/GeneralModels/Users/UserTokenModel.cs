namespace VardoneEntities.Models.GeneralModels.Users
{
    public record UserTokenModel
    {
        public long UserId { get; set; }
        public string Token { get; set; }
    }
}