using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("private_chats")]
    public class PrivateChatsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("from_id"), ForeignKey("from_id"), Required] public virtual UsersTable FromUser { get; set; }
        [Column("to_id"), ForeignKey("to_id"), Required] public virtual UsersTable ToUser { get; set; }
    }
}