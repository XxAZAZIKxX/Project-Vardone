using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.Guilds
{
    [Table("guild_members")]
    public sealed class GuildMembersTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("user_id"), ForeignKey("user_id"), Required] public UsersTable User { get; set; }
        [Column("guild_id"), ForeignKey("guild_id"), Required] public GuildsTable Guild { get; set; }
        [Column("join_date"), Required] public DateTime JoinDate { get; set; }
        [Column("number_of_invited_members"), Required] public int NumberOfInvitedMembers { get; set; }
    }
}