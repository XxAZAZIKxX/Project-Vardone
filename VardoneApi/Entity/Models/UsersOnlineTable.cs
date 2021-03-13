using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("users_online")]
    public class UsersOnlineTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("user_id"), ForeignKey("user_id"), Required] public virtual UsersTable User { get; set; }
        [Column("last_online"), Required] public DateTime LastOnlineTime { get; set; }
    }
}