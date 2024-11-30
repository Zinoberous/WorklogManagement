namespace WorklogManagement.Shared.Models;

public class WorkTime : IDataModel
{
    public int? Id { get; init; }

    public required Enums.WorkTimeType Type { get; init; }

    public required DateOnly Date { get; init; }

    public required int ExpectedMinutes { get; init; }

    public required int ActualMinutes { get; init; }

    public string? Note { get; init; }
}
