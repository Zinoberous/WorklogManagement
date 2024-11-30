using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.Data.Models;

[Table("Worklog")]
public partial class Worklog
{
    [Key]
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public int TicketId { get; set; }

    public string? Description { get; set; }

    public TimeOnly TimeSpent { get; set; }

    public int TimeSpentSeconds { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("TicketId")]
    [InverseProperty("Worklogs")]
    public virtual Ticket Ticket { get; set; } = null!;

    [InverseProperty("Worklog")]
    public virtual ICollection<WorklogAttachment> WorklogAttachments { get; set; } = new List<WorklogAttachment>();
}
