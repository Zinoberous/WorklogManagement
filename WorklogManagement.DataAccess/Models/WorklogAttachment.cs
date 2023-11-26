using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models;

[Table("WorklogAttachment")]
[Index("WorklogId", "Name", Name = "UX_WorklogAttachment_WorklogId_Name", IsUnique = true)]
public partial class WorklogAttachment
{
    [Key]
    public int Id { get; set; }

    public int WorklogId { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    public string? Comment { get; set; }

    [ForeignKey("WorklogId")]
    [InverseProperty("WorklogAttachments")]
    public virtual Worklog Worklog { get; set; } = null!;
}
