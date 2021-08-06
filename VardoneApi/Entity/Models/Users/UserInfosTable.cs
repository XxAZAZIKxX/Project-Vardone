using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Users
{
    [Table("user_infos")]
    public sealed class UserInfosTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("id_user"), ForeignKey("id_user"), Required] public UsersTable User { get; set; }
        [Column("avatar")] public byte[] Avatar { get; set; } 
        [Column("description")] public string Description { get; set; }
    }
}