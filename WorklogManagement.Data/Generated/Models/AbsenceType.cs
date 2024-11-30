using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.Data.Models;

[Table("AbsenceType")]
[Index("Name", Name = "UX_AbsenceType_Name", IsUnique = true)]
public partial class AbsenceType
{
    [Key]
    public int Id { get; set; }

    [StringLength(255)]
    public string Name { get; set; } = null!;

    public byte[] RowVersion { get; set; } = null!;

    [InverseProperty("AbsenceType")]
    public virtual ICollection<Absence> Absences { get; set; } = new List<Absence>();
}
