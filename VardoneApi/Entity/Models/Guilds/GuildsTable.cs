using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.Guilds
{
    [Table("guilds")]
    public sealed class GuildsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("name"), Required] public string Name { get; set; }
        [Column("owner_id"), ForeignKey("owner_id"), Required] public UsersTable Owner { get; set; }
        [Column("created_at"), Required] public DateTime CreatedAt { get; set; }
        [Column("info_id"), ForeignKey("info_id")] public GuildInfosTable Info { get; set; }
    }
}