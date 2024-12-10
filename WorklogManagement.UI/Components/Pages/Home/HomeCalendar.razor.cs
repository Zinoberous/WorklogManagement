using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.Home;

public partial class HomeCalendar
{
    [Parameter]
    public required int Year { get; set; }

    [Parameter]
    public IEnumerable<WorkTime> WorkTimes { get; set; } = [];

    [Parameter]
    public IEnumerable<Absence> Absences { get; set; } = [];

    [Parameter]
    public IEnumerable<Holiday> Holidays { get; set; } = [];
}
