namespace VardoneEntities.Models.GeneralModels.Users
{
    public class RegisterUserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}