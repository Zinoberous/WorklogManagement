namespace WorklogManagement.API.Models;

internal record CalendarEntry
{
    internal required string Type { get; init; }
    internal required int TypeId { get; init; }
    internal required DateOnly Date { get; init; }
    internal required int DurationMinutes { get; init; }
}
