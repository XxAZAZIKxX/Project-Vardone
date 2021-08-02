using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("guilds")]
    public class GuildsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("name"), Required] public string Name { get; set; }
        [Column("owner_id"), ForeignKey("owner_id"), Required] public virtual UsersTable Owner { get; set; }
        [Column("created_at"), Required] public DateTime CreatedAt { get; set; }
    }
}