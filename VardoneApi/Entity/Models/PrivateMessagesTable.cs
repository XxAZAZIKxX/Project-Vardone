using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("private_messages")]
    public class PrivateMessagesTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("chat_id"), ForeignKey("chat_id"), Required] public virtual PrivateChatsTable Chat { get; set; }
        [Column("from_id"), ForeignKey("from_id"), Required] public virtual UsersTable From { get; set; }
        [Column("created_time"), Required] public DateTime CreatedTime { get; set; }
        [Column("text"), Required] public string Text { get; set; }
        [Column("image")] public byte[] Image { get; set; }
    }
}