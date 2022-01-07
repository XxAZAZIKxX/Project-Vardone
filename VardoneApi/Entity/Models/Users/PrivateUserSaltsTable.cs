using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models.Users
{
    [Table("private_user_salts")]
    public sealed class PrivateUserSaltsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("user_id"), Required, ForeignKey("user_id")] public UsersTable User { get; set; }
        [Column("pus"), Required] public string Pus { get; set; }
    }
}