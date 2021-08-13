using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.Channels
{
    [Table("channel_messages")]
    public sealed class ChannelMessagesTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("channel_id"), ForeignKey("channel_id"), Required] public ChannelsTable Channel { get; set; }
        [Column("author_id"), ForeignKey("author_id"), Required] public UsersTable Author { get; set; }
        [Column("created_time"), Required] public DateTime CreatedTime { get; set; }
        [Column("text"), Required] public string Text { get; set; }
        [Column("image")] public byte[] Image { get; set; }
    }
}