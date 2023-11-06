using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("TicketCommentAttachment")]
    [Index("TicketCommentId", "Name", Name = "UX_TicketCommentAttachment_TicketCommentId_Name", IsUnique = true)]
    public partial class TicketCommentAttachment
    {
        [Key]
        public int Id { get; set; }
        public int TicketCommentId { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;
        public string Comment { get; set; } = null!;

        [ForeignKey("TicketCommentId")]
        [InverseProperty("TicketCommentAttachments")]
        public virtual TicketComment TicketComment { get; set; } = null!;
    }
}
