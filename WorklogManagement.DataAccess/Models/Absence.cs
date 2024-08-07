﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WorklogManagement.DataAccess.Models;

[Table("Absence")]
public partial class Absence
{
    [Key]
    public int Id { get; set; }

    public int AbsenceTypeId { get; set; }

    public DateOnly Date { get; set; }

    public int DurationMinutes { get; set; }

    public string? Note { get; set; }

    [ForeignKey("AbsenceTypeId")]
    [InverseProperty("Absences")]
    public virtual AbsenceType AbsenceType { get; set; } = null!;
}
