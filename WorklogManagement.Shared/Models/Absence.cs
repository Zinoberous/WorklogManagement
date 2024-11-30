namespace WorklogManagement.Shared.Models;

public class Absence : IDataModel
{
    public int? Id { get; init; }

    public required Enums.AbsenceType Type { get; init; }

    public required DateOnly Date { get; init; }

    public required int DurationMinutes { get; init; }

    public string? Note { get; init; }
}
