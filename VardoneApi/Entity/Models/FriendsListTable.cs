using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("friends_list")]
    public class FriendsListTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("from_id"), ForeignKey("from_id"), Required] public virtual UsersTable FromUser { get; set; }
        [Column("to_id"), ForeignKey("to_id"), Required] public virtual UsersTable ToUser { get; set; }
        [Column("confirmed"), Required] public bool Confirmed { get; set; } = false;
    }
}