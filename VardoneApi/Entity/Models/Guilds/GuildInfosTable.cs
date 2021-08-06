using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Guilds
{
    [Table("guild_infos")]
    public sealed class GuildInfosTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("guild_id"), ForeignKey("guild_id"), Required] public GuildsTable Guild { get; set; }
        [Column("avatar")] public byte[] Avatar { get; set; }
    }
}