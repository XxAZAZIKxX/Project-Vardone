namespace VardoneEntities.Entities
{
    public class PrivateMessage
    {
        public long MessageId { get; init; }
        public PrivateChat Chat { get; init; }
        public User Author { get; init; }
        public string Text { get; init; }
        public string Base64Image { get; init; }
    }
}