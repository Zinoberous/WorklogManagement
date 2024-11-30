using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Models;
using static WorklogManagement.UI.Helper.DisplayHelper;

namespace WorklogManagement.UI.Components.Pages.Home;
public partial class HomeCalendarDay
{
    [Parameter]
    public required DateOnly Date { get; set; }

    [Parameter]
    public required IEnumerable<WorkTime> WorkTimes { get; set; }

    [Parameter]
    public required IEnumerable<Absence> Absences { get; set; }

    [Parameter]
    public Holiday? Holiday { get; set; }

    private static readonly string[] _dayLabelShorts = ["So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"];

    private static readonly string _timeCompensationIcon = "timer_off";

    private static readonly Dictionary<WorkTimeType, string> _workTimeIcons = new()
    {
        { WorkTimeType.Office, "business" },
        { WorkTimeType.Mobile, "home" },
    };

    private static readonly Dictionary<AbsenceType, string> _absenceIcons = new()
    {
        { AbsenceType.Holiday, "celebration" },
        { AbsenceType.Vacation, "beach_access" },
        { AbsenceType.Ill, "pill" },
    };

    private string GetDayColoring()
    {
        var color = "unset";
        ICollection<string> background = [];

        if (Date == DateOnly.FromDateTime(DateTime.Today))
        {
            color = "black";
            background.Add("lightgreen");
        }

        if (Holiday is not null)
        {
            color = "black";
            background.Add("lightblue");
        }

        if (new[] { DayOfWeek.Saturday, DayOfWeek.Sunday }.Contains(Date.DayOfWeek))
        {

            color = "black";
            background.Add("darkgray");
        }

        return $"color: {color}; background: {(background.Count > 1 ? $"linear-gradient(to right, {string.Join(", ", background)})" : background.Count == 1 ? background.ElementAt(0) : "unset")};";
    }

    private string GetWeekDayClassName()
    {
        var actualMinutes = WorkTimes.Sum(x => x.ActualMinutes);
        var expectedMinutes = WorkTimes.Sum(x => x.ExpectedMinutes);

        if (actualMinutes > expectedMinutes)
        {
            return "overtime";
        }
        else if (actualMinutes < expectedMinutes)
        {
            return "undertime";
        }

        return string.Empty;
    }

    private string GetWeekDayTitle()
    {
        var actualMinutes = WorkTimes.Sum(x => x.ActualMinutes);
        var expectedMinutes = WorkTimes.Sum(x => x.ExpectedMinutes);

        if (actualMinutes > expectedMinutes)
        {
            return $"Überstunden: {MinutesToTime(actualMinutes - expectedMinutes)}";
        }
        else if (actualMinutes < expectedMinutes)
        {
            return $"Minusstunden: {MinutesToTime(expectedMinutes - actualMinutes)}";
        }

        return string.Empty;
    }

    private static string GetIconTitle(string label, int durationMinutes, string? note)
    {
        return $"{label}: {MinutesToTime(durationMinutes)}{(string.IsNullOrWhiteSpace(note) ? string.Empty : $"\n{note}")}";
    }
}
