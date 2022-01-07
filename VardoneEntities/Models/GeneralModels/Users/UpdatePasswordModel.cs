namespace VardoneEntities.Models.GeneralModels.Users
{
    public class UpdatePasswordModel
    {
        public string PreviousPasswordHash { get; set; }
        public string NewPasswordHash { get; set; }
    }
}