namespace WorklogManagement.Shared.Models;

public class Absence : IDataModel
{
    public int? Id { get; init; }

    public required Enums.AbsenceType Type { get; set; }

    public required DateOnly Date { get; init; }

    public required int DurationMinutes { get; set; }

    public string? Note { get; set; }
}
