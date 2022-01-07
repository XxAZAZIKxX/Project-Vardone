namespace VardoneEntities.Models.ApiModels
{
    public class GetUserTokenApiModel
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string MacAddress { get; set; }
    }
}