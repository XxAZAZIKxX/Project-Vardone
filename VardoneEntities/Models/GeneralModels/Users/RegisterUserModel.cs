namespace VardoneEntities.Models.GeneralModels.Users
{
    public record RegisterUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}