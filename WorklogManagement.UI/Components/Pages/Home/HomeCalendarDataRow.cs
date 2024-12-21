namespace WorklogManagement.UI.Components.Pages.Home;

public record HomeCalendarDataRow
{
    public required int Year { get; set; }
    public required int Month { get; init; }
    public required IEnumerable<HomeCalendarDataCell> Days { get; init; }
}
