using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.DataAccess.Models;

[Table("CalendarEntry")]
[Index("Date", "CalendarEntryTypeId", Name = "UX_CalendarEntry_Date_CalendarEntryTypeId", IsUnique = true)]
public partial class CalendarEntry
{
    [Key]
    public int Id { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly Duration { get; set; }

    public int CalendarEntryTypeId { get; set; }

    public string? Note { get; set; }

    [ForeignKey("CalendarEntryTypeId")]
    [InverseProperty("CalendarEntries")]
    public virtual CalendarEntryType CalendarEntryType { get; set; } = null!;
}
