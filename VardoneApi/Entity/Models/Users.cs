using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("users")]
    public class Users
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("username"), Required] public string Username { get; set; }
        [Column("password"), Required] public string Password { get; set; }
        [Column("email"), Required] public string Email { get; set; }
        [Column("user_info_id"), ForeignKey("user_info_id")] public UserInfos Info { get; set; } = null;
    }
}