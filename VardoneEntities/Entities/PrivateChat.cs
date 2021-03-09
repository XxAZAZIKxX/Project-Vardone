namespace VardoneEntities.Entities
{
    public class PrivateChat
    {
        public long ChatId { get; init; }
        public User FromUser { get; init; }
        public User ToUser { get; init; }
    }
}