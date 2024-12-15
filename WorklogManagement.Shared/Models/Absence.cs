namespace WorklogManagement.Shared.Models;

public record Absence : IDataModel
{
    public int Id { get; init; }

    public required Enums.AbsenceType Type { get; set; }

    public required DateOnly Date { get; init; }

    public required TimeSpan Duration { get; set; }

    public string? Note { get; set; }
}
