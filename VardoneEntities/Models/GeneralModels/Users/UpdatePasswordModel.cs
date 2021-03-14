namespace VardoneEntities.Models.GeneralModels.Users
{
    public class UpdatePasswordModel
    {
        public string PreviousPassword { get; set; }
        public string NewPassword { get; set; }
    }
}