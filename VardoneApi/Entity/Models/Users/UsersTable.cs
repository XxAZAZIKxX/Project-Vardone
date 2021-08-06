using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Users
{
    [Table("users")]
    public sealed class UsersTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("username"), Required] public string Username { get; set; }
        [Column("password"), Required] public string Password { get; set; }
        [Column("email"), Required] public string Email { get; set; }
        [Column("info_id"), ForeignKey("info_id")] public UserInfosTable Info { get; set; }
    }
}