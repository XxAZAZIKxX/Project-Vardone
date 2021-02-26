using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("users")]
    public class Users
    {
        [Column("id"), Key]  public int Id { get; set; }
        [Column("username"), Required]  public string Username { get; set; }
        [Column("email"), Required]  public string Email { get; set; }
        [Column("password"), Required]  public string Password { get; set; }
    }
}