using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("TicketStatus")]
    [Index("Name", Name = "UX_TicketStatus_Name", IsUnique = true)]
    public partial class TicketStatus
    {
        public TicketStatus()
        {
            TicketStatusLogs = new HashSet<TicketStatusLog>();
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [InverseProperty("TicketStatus")]
        public virtual ICollection<TicketStatusLog> TicketStatusLogs { get; set; }
        [InverseProperty("TicketStatus")]
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
