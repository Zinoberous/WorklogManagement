﻿using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models;

[Table("TicketAttachment")]
[Index("TicketId", "Name", Name = "UX_TicketAttachment_TicketId_Name", IsUnique = true)]
public partial class TicketAttachment
{
    [Key]
    public int Id { get; set; }

    public int TicketId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    public string? Comment { get; set; }

    [ForeignKey("TicketId")]
    [InverseProperty("TicketAttachments")]
    public virtual Ticket Ticket { get; set; } = null!;
}
