using Microsoft.AspNetCore.Components;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.Components.Home;
public partial class HomeCalendarDay
{
    [Parameter]
    public required DateOnly Date { get; set; }

    [Parameter]
    public required IEnumerable<WorkTime> WorkTimes { get; set; }

    [Parameter]
    public required IEnumerable<Absence> Absences { get; set; }

    [Parameter]
    public required IEnumerable<Holiday> Holidays { get; set; }

    private static readonly string[] _dayLabelShorts = ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"];
}
