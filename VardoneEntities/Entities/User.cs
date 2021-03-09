namespace VardoneEntities.Entities
{
    public class User
    {
        public long Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Base64Avatar { get; init; }
        public string Description { get; init; }
    }
}