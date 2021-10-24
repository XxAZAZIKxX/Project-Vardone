namespace VardoneEntities.Entities.Guild
{
    public class BannedUser
    {
        public User User { get; set; }
        public string Reason { get; set; }
    }
}