using Microsoft.AspNetCore.Components;
using WorklogManagement.Service.Models;
using WorklogManagement.UI.Models;

namespace WorklogManagement.UI.Components.Home;

public partial class HomeCalendar
{
    [Parameter]
    public required int Year { get; set; }

    [Parameter]
    public required IEnumerable<WorkTime> WorkTimes { get; set; }

    [Parameter]
    public required IEnumerable<Absence> Absences { get; set; }

    [Parameter]
    public required IEnumerable<Holiday> Holidays { get; set; }

    private static readonly string[] _monthLabelShorts = ["Jan", "Feb", "Mär", "Apr", "Mai", "Jun", "Jul", "Aug", "Sep", "Okt", "Nov", "Dez"];
}
