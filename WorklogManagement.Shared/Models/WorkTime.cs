namespace WorklogManagement.Shared.Models;

public class WorkTime : IDataModel
{
    public int? Id { get; init; }

    public required Enums.WorkTimeType Type { get; set; }

    public required DateOnly Date { get; init; }

    public required int ExpectedMinutes { get; set; }

    public required int ActualMinutes { get; set; }

    public string? Note { get; set; }
}