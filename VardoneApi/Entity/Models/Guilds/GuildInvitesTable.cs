using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;

namespace VardoneApi.Entity.Models.Guilds
{
    [Table("guild_invites")]
    public sealed class GuildInvitesTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("guild_id"), ForeignKey("guild_id"), Required] public GuildsTable Guild { get; set; }
        [Column("number_of_uses")] public int NumberOfUses { get; set; }
        [Column("created_by_user_id"), ForeignKey("created_by_user_id"), Required] public UsersTable CreatedByUser { get; set; }
        [Column("created_at"), Required] public DateTime CreatedAt { get; set; }
        [Column("invite_code"), Required] public string InviteCode { get; set; }
    }
}