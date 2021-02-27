using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VardoneApi.Entity.Models
{
    public class UserInfos
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("id_user"), ForeignKey("id_user"), Required] public Users User { get; set; }
        [Column("avatar")] public byte[] Avatar { get; set; } = null;
        [Column("description")] public string Description { get; set; } = null;
    }
}