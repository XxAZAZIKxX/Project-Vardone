using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("friends_list")]
    public class FriendsListTable
    {
        [Column("id"), Key] public long FriendListId { get; set; }
        [Column("from_id"), ForeignKey("from_id"), Required] public virtual UsersTable FromUser { get; set; }
        [Column("to_id"), ForeignKey("to_id"), Required] public virtual UsersTable ToUser { get; set; }
        [Column("createdBy_id"), ForeignKey("createdBy_id"), Required] public virtual UsersTable CreatedByUser { get; set; }
        [Column("confirmed"), Required] public bool Confirmed { get; set; } = false;
    }
}