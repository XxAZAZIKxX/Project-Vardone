using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    [Table("tokens")]
    public class TokensTable
    {
        [Column("id"), Key] public long TokenId { get; set; }
        [Column("id_user"), ForeignKey("id_user"), Required] public virtual UsersTable User { get; set; }
        [Column("token"), Required] public string Token { get; set; }
        [Column("created_at"), Required] public DateTime CreatedAt { get; set; }
        [Column("ip_address"), Required] public string IpAddress { get; set; }
        [Column("mac_address"), Required] public string MacAddress { get; set; }
    }
}