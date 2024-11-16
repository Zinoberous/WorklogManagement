using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.Data.Models;

[Table("TicketStatus")]
[Index("Name", Name = "UX_TicketStatus_Name", IsUnique = true)]
public partial class TicketStatus
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [InverseProperty("TicketStatus")]
    public virtual ICollection<TicketStatusLog> TicketStatusLogs { get; set; } = new List<TicketStatusLog>();

    [InverseProperty("TicketStatus")]
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
