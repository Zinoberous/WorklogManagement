using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models
{
    [Table("TicketComment")]
    public partial class TicketComment
    {
        [Key]
        public int Id { get; set; }
        public int TicketId { get; set; }
        public string Comment { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("TicketId")]
        [InverseProperty("TicketComments")]
        public virtual Ticket Ticket { get; set; } = null!;
    }
}
