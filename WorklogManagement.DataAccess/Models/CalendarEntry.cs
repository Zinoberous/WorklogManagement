using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("CalendarEntry")]
    [Index("Date", "CalendarEntryTypeId", Name = "UX_Day_Date_CalendarEntryTypeId", IsUnique = true)]
    public partial class CalendarEntry
    {
        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public int CalendarEntryTypeId { get; set; }
        public string? Note { get; set; }

        [ForeignKey("CalendarEntryTypeId")]
        [InverseProperty("CalendarEntries")]
        public virtual CalendarEntryType CalendarEntryType { get; set; } = null!;
    }
}
