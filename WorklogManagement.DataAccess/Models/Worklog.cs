using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("Worklog")]
    public partial class Worklog
    {
        public Worklog()
        {
            WorklogAttachments = new HashSet<WorklogAttachment>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public int TicketId { get; set; }
        public string? Description { get; set; }
        public TimeSpan TimeSpent { get; set; }

        [ForeignKey("TicketId")]
        [InverseProperty("Worklogs")]
        public virtual Ticket Ticket { get; set; } = null!;
        [InverseProperty("Worklog")]
        public virtual ICollection<WorklogAttachment> WorklogAttachments { get; set; }
    }
}
