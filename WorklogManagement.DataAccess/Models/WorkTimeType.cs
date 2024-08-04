using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models;

[Table("WorkTimeType")]
[Index("Name", Name = "UX_WorkTimeType_Name", IsUnique = true)]
public partial class WorkTimeType
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    [InverseProperty("WorkTimeType")]
    public virtual ICollection<WorkTime> WorkTimes { get; set; } = new List<WorkTime>();
}
