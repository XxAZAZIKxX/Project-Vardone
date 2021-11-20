using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Guilds;

namespace VardoneApi.Entity.Models.Channels
{
    [Table("channels")]
    public sealed class ChannelsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("guild_id"), ForeignKey("guild_id"), Required] public GuildsTable Guild { get; set; }
        [Column("name"), Required] public string Name { get; set; }
        [Column("last_delete_message_time")] public DateTime? LastDeleteMessageTime { get; set; }
    }
}