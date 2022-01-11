namespace VardoneEntities.Models.GeneralModels.Users
{
    public record UpdatePasswordModel
    {
        public string PreviousPasswordHash { get; set; }
        public string NewPasswordHash { get; set; }
    }
}