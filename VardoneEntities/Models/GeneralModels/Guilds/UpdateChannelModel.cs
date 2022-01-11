namespace VardoneEntities.Models.GeneralModels.Guilds
{
    public record UpdateChannelModel
    {
        public long ChannelId { get; set; }
        public string Name { get; set; }
    }
}