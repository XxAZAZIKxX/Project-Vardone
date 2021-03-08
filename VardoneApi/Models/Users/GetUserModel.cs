namespace VardoneApi.Models.Users
{
    public class GetUserModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Base64Avatar { get; set; }
        public string Description { get; set; }
    }
}