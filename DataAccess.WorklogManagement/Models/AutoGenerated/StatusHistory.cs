using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.WorklogManagement.Models
{
    [Table("StatusHistory")]
    public partial class StatusHistory
    {
        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public int StatusId { get; set; }
        public string? Comment { get; set; }

        [ForeignKey("StatusId")]
        [InverseProperty("StatusHistories")]
        public virtual Status Status { get; set; } = null!;
        [ForeignKey("TicketId")]
        [InverseProperty("StatusHistories")]
        public virtual Ticket Ticket { get; set; } = null!;
    }
}
