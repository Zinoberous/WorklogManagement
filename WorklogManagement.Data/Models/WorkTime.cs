using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.Data.Models;

[Table("WorkTime")]
public partial class WorkTime
{
    [Key]
    public int Id { get; set; }

    public int WorkTimeTypeId { get; set; }

    public DateOnly Date { get; set; }

    public int ExpectedMinutes { get; set; }

    public int ActualMinutes { get; set; }

    public string? Note { get; set; }

    [ForeignKey("WorkTimeTypeId")]
    [InverseProperty("WorkTimes")]
    public virtual WorkTimeType WorkTimeType { get; set; } = null!;
}
