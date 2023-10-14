using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models
{
    [Table("Attachment")]
    [Index("WorklogId", "Name", Name = "UX_Attachment_Name", IsUnique = true)]
    public partial class Attachment
    {
        [Key]
        public int Id { get; set; }
        public int WorklogId { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;
        public string Comment { get; set; } = null!;

        [ForeignKey("WorklogId")]
        [InverseProperty("Attachments")]
        public virtual Worklog Worklog { get; set; } = null!;
    }
}
