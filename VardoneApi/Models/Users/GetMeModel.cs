namespace VardoneApi.Models.Users
{
    public class GetMeModel
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Base64Avatar { get; set; }
        public string Description { get; set; }
    }
}