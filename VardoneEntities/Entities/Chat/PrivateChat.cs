namespace VardoneEntities.Entities.Chat
{
    /// <summary>
    /// Представляет объект приватного чата
    /// </summary>
    public record PrivateChat
    {
        public long ChatId { get; set; } = -1;
        public User.User FromUser { get; set; }
        public User.User ToUser { get; set; }
        public int UnreadMessages { get; set; }
    }
}