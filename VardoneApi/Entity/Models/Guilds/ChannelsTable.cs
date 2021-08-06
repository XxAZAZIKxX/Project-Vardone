using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Guilds
{
    [Table("channels")]
    public sealed class ChannelsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("guild_id"), ForeignKey("guild_id"), Required] public GuildsTable Guild { get; set; }
        [Column("name"), Required] public string Name { get; set; }
    }
}