using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models
{
    [Table("Status")]
    [Index("Name", Name = "UX_Status_Name", IsUnique = true)]
    public partial class Status
    {
        public Status()
        {
            Tickets = new HashSet<Ticket>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [InverseProperty("Status")]
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
