namespace WorklogManagement.API.Statistics;

internal record CalendarEntry
{
    internal required string Type { get; init; }
    internal required int TypeId { get; init; }
    internal required DateOnly Date { get; init; }
    internal required int DurationSeconds { get; init; }
}
