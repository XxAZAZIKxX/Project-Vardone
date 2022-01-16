namespace VardoneEntities.Entities.User
{
    public record User
    {
        public long UserId { get; set; }
        public string Username { get; set; }
        public string Base64Avatar { get; set; }
        public string Description { get; set; }
        public AdditionalUserInformation AdditionalInformation { get; set; }
    }
}