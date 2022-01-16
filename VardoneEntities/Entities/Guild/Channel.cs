namespace VardoneEntities.Entities.Guild
{
    public record Channel
    {
        public long ChannelId { get; set; } = -1;
        public string Name { get; set; }
        public Guild Guild { get; set; }
    }
}