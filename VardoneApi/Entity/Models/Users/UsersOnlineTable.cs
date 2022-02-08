using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Users
{
    [Table("users_online")]
    public sealed class UsersOnlineTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("user_id"), ForeignKey("user_id"), Required] public UsersTable User { get; set; }
        [Column("is_online"), Required] public bool IsOnline { get; set; }
    }
}