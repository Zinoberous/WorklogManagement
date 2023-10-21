using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models
{
    [Table("TicketAttachment")]
    [Index("TicketId", "Name", Name = "UX_TicketAttachment_Name", IsUnique = true)]
    public partial class TicketAttachment
    {
        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;
        public string Comment { get; set; } = null!;

        [ForeignKey("TicketId")]
        [InverseProperty("TicketAttachments")]
        public virtual Ticket Ticket { get; set; } = null!;
    }
}
