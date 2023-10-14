using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.WorklogManagement.Models
{
    [Table("Worklog")]
    public partial class Worklog
    {
        public Worklog()
        {
            Attachments = new HashSet<Attachment>();
        }

        [Key]
        public int Id { get; set; }
        public int DayId { get; set; }
        public int TicketId { get; set; }
        public string? Description { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public string? TimeSpentComment { get; set; }

        [ForeignKey("DayId")]
        [InverseProperty("Worklogs")]
        public virtual Day Day { get; set; } = null!;
        [ForeignKey("TicketId")]
        [InverseProperty("Worklogs")]
        public virtual Ticket Ticket { get; set; } = null!;
        [InverseProperty("Worklog")]
        public virtual ICollection<Attachment> Attachments { get; set; }
    }
}
