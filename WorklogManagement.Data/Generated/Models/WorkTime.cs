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

    public int ExpectedSeconds { get; set; }

    public int ActualSeconds { get; set; }

    public string? Note { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("WorkTimeTypeId")]
    [InverseProperty("WorkTimes")]
    public virtual WorkTimeType WorkTimeType { get; set; } = null!;
}
