using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("CalendarEntryType")]
    [Index("Name", Name = "UX_CalendarEntryType_Name", IsUnique = true)]
    public partial class CalendarEntryType
    {
        public CalendarEntryType()
        {
            CalendarEntries = new HashSet<CalendarEntry>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [InverseProperty("CalendarEntryType")]
        public virtual ICollection<CalendarEntry> CalendarEntries { get; set; }
    }
}
