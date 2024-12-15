namespace WorklogManagement.Shared.Models;

public record WorkTime : IDataModel
{
    public int Id { get; init; }

    public required Enums.WorkTimeType Type { get; set; }

    public required DateOnly Date { get; init; }

    public required TimeSpan Expected { get; set; }

    public required TimeSpan Actual { get; set; }

    public string? Note { get; set; }
}
