using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.PrivateChats
{
    [Table("private_messages")]
    public sealed class PrivateMessagesTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("chat_id"), ForeignKey("chat_id"), Required] public PrivateChatsTable Chat { get; set; }
        [Column("from_id"), ForeignKey("from_id"), Required] public UsersTable Author { get; set; }
        [Column("created_time"), Required] public DateTime CreatedTime { get; set; }
        [Column("text"), Required] public string Text { get; set; }
        [Column("image")] public byte[] Image { get; set; }
    }
}