namespace VardoneApi.Models.Users
{
    public class LoginUserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string MacAddress { get; set; }
        public string IpAddress { get; set; }
    }
}