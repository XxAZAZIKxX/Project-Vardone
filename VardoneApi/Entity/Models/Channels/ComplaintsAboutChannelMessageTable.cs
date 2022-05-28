using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using VardoneApi.Entity.Models.Users;
using VardoneEntities.Models.GeneralModels;

namespace VardoneApi.Entity.Models.Channels
{
    [Table("complaints_about_channel_message")]
    public class ComplaintsAboutChannelMessageTable
    {
        [Column("id"), Key] public long Id { get; set; }
        [Column("message_id"), Required] public long MessageId { get; set; }
        [Column("compainant_id"), ForeignKey("compainant_id"), Required] public UsersTable Complainant { get; set; }
        [Column("complaint_type"), Required] public ComplaintType ComplaintType { get; set; }
        [Column("subject_of_the_complaint_id"), ForeignKey("subject_of_the_complaint_id"), Required] public UsersTable SubjectOfTheComplaint { get; set; }
        [Column("message_text")] public string MessageText { get; set; }
        [Column("message_image")] public byte[] MessageImage { get; set; }
    }
}