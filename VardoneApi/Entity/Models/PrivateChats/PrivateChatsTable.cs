using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.PrivateChats
{
    [Table("private_chats")]
    public sealed class PrivateChatsTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("from_id"), ForeignKey("from_id"), Required] public UsersTable FromUser { get; set; }
        [Column("to_id"), ForeignKey("to_id"), Required] public UsersTable ToUser { get; set; }
        [Column("from_last_readTime_messages"), Required] public DateTime FromLastReadTimeMessages { get; set; }
        [Column("to_last_readTime_messages"), Required] public DateTime ToLastReadTimeMessages { get; set; }
    }
}