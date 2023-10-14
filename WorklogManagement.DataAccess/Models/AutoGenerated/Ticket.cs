using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models
{
    [Table("Ticket")]
    [Index("Title", Name = "UX_Ticket_Title", IsUnique = true)]
    public partial class Ticket
    {
        public Ticket()
        {
            InverseRef = new HashSet<Ticket>();
            StatusHistories = new HashSet<StatusHistory>();
            Worklogs = new HashSet<Worklog>();
        }

        [Key]
        public int Id { get; set; }
        public int? RefId { get; set; }
        [StringLength(255)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int StatusId { get; set; }

        [ForeignKey("RefId")]
        [InverseProperty("InverseRef")]
        public virtual Ticket? Ref { get; set; }
        [ForeignKey("StatusId")]
        [InverseProperty("Tickets")]
        public virtual Status Status { get; set; } = null!;
        [InverseProperty("Ref")]
        public virtual ICollection<Ticket> InverseRef { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<StatusHistory> StatusHistories { get; set; }
        [InverseProperty("Ticket")]
        public virtual ICollection<Worklog> Worklogs { get; set; }
    }
}
