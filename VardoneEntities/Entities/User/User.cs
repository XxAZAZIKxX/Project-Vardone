namespace VardoneEntities.Entities.User
{
    public class User
    {
        public long UserId { get; init; }
        public string Username { get; init; }
        public string Base64Avatar { get; init; }
        public string Description { get; init; }
        public AdditionalUserInformation AdditionalInformation { get; init; }
    }
}