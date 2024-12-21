using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.Home;

public record HomeCalendarDataCell
{
    public required DateOnly Date { get; init; }

    public required IEnumerable<WorkTime> WorkTimes { get; init; }

    public required IEnumerable<Absence> Absences { get; init; }

    public Holiday? Holiday { get; init; }
}
