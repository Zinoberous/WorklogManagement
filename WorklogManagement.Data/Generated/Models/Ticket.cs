using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.Data.Models;

[Table("Ticket")]
public partial class Ticket
{
    [Key]
    public int Id { get; set; }

    public int? RefId { get; set; }

    [StringLength(255)]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int TicketStatusId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [InverseProperty("Ref")]
    public virtual ICollection<Ticket> InverseRef { get; set; } = new List<Ticket>();

    [ForeignKey("RefId")]
    [InverseProperty("InverseRef")]
    public virtual Ticket? Ref { get; set; }

    [InverseProperty("Ticket")]
    public virtual ICollection<TicketAttachment> TicketAttachments { get; set; } = new List<TicketAttachment>();

    [ForeignKey("TicketStatusId")]
    [InverseProperty("Tickets")]
    public virtual TicketStatus TicketStatus { get; set; } = null!;

    [InverseProperty("Ticket")]
    public virtual ICollection<TicketStatusLog> TicketStatusLogs { get; set; } = new List<TicketStatusLog>();

    [InverseProperty("Ticket")]
    public virtual ICollection<Worklog> Worklogs { get; set; } = new List<Worklog>();
}
