using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("Ticket")]
    [Index("Title", Name = "UX_Ticket_Title", IsUnique = true)]
    public partial class Ticket
    {
        public Ticket()
        {
            InverseRef = new HashSet<Ticket>();
            TicketAttachments = new HashSet<TicketAttachment>();
            TicketStatusLogs = new HashSet<TicketStatusLog>();
            Worklogs = new HashSet<Worklog>();
        }

        [Key]
        public int Id { get; set; }
        public int? RefId { get; set; }
        [StringLength(255)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int TicketStatusId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("RefId")]
        [InverseProperty("InverseRef")]
        public virtual Ticket? Ref { get; set; }
        [ForeignKey("TicketStatusId")]
        [InverseProperty("Tickets")]
        public virtual TicketStatus TicketStatus { get; set; } = null!;
        [InverseProperty("Ref")]
        public virtual ICollection<Ticket> InverseRef { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<TicketStatusLog> TicketStatusLogs { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<Worklog> Worklogs { get; set; }
    }
}
