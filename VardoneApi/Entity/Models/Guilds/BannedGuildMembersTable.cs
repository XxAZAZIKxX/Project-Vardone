using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.Guilds
{
    [Table("banned_guild_members")]
    public sealed class BannedGuildMembersTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("guild_id"), ForeignKey("guild_id"), Required] public GuildsTable Guild { get; set; }
        [Column("banned_user_id"), ForeignKey("banned_user_id"), Required] public UsersTable BannedUser { get; set; }
        [Column("banned_by_user_id"), ForeignKey("banned_by_user_id"), Required] public UsersTable BannedByUser { get; set; }
        [Column("reason")] public string Reason { get; set; }
        [Column("ban_date"), Required] public DateTime BanDate { get; set; }
    }
}