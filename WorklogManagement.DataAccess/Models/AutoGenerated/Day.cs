using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models
{
    [Table("Day")]
    [Index("Date", "IsMobile", Name = "UX_Day_Date_Mobile", IsUnique = true)]
    public partial class Day
    {
        public Day()
        {
            Worklogs = new HashSet<Worklog>();
        }

        [Key]
        public int Id { get; set; }
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        public bool IsMobile { get; set; }
        public int? WorkloadId { get; set; }
        public string? WorkloadComment { get; set; }

        [ForeignKey("WorkloadId")]
        [InverseProperty("Days")]
        public virtual Workload? Workload { get; set; }
        [InverseProperty("Day")]
        public virtual ICollection<Worklog> Worklogs { get; set; }
    }
}
