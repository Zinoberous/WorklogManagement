using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("TicketComment")]
    public partial class TicketComment
    {
        public TicketComment()
        {
            TicketCommentAttachments = new HashSet<TicketCommentAttachment>();
        }

        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Description { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("TicketId")]
        [InverseProperty("TicketComments")]
        public virtual Ticket Ticket { get; set; } = null!;

        [InverseProperty("TicketComment")]
        public virtual ICollection<TicketCommentAttachment> TicketCommentAttachments { get; set; }
    }
}
