namespace VardoneEntities.Entities
{
    public class BannedUser
    {
        public User User { get; init; }
        public string Reason { get; init; }
    }
}