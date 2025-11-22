using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WorklogManagement.Data.Models;

[Table("Absence")]
public partial class Absence
{
    [Key]
    public int Id { get; set; }

    public int AbsenceTypeId { get; set; }

    public DateOnly Date { get; set; }

    public int DurationSeconds { get; set; }

    public string? Note { get; set; }

    public byte[] RowVersion { get; set; } = null!;

    [ForeignKey("AbsenceTypeId")]
    [InverseProperty("Absences")]
    public virtual AbsenceType AbsenceType { get; set; } = null!;
}
