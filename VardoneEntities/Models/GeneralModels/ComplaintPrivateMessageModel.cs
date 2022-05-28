namespace VardoneEntities.Models.GeneralModels
{
    public enum ComplaintType
    {
        Insult, Porn, Spam, Violence
    }

    public record ComplaintMessageModel
    {
        public long MessageId { get; set; }
        public ComplaintType ComplaintType { get; set; }
    };
}