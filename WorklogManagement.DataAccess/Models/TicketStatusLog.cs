using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models;

[Table("TicketStatusLog")]
public partial class TicketStatusLog
{
    [Key]
    public int Id { get; set; }

    public int TicketId { get; set; }

    public int TicketStatusId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartedAt { get; set; }

    public string? Note { get; set; }

    [ForeignKey("TicketId")]
    [InverseProperty("TicketStatusLogs")]
    public virtual Ticket Ticket { get; set; } = null!;

    [ForeignKey("TicketStatusId")]
    [InverseProperty("TicketStatusLogs")]
    public virtual TicketStatus TicketStatus { get; set; } = null!;
}
