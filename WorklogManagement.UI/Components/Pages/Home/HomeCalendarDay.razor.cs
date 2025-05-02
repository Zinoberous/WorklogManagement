using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Extensions;

namespace WorklogManagement.UI.Components.Pages.Home;
public partial class HomeCalendarDay
{
    [Parameter]
    public required DateOnly Date { get; set; }

    [Parameter]
    public IEnumerable<WorkTime> WorkTimes { get; set; } = [];

    [Parameter]
    public IEnumerable<Absence> Absences { get; set; } = [];

    [Parameter]
    public Holiday? Holiday { get; set; }

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
        var actual = TimeSpan.FromTicks(WorkTimes.Sum(x => x.Actual.Ticks));
        var expected = TimeSpan.FromTicks(WorkTimes.Sum(x => x.Expected.Ticks));

        if (actual > expected)
        {
            return "overtime";
        }
        else if (actual < expected)
        {
            return "undertime";
        }

        return string.Empty;
    }

    private string GetWeekDayTitle()
    {
        var actual = TimeSpan.FromTicks(WorkTimes.Sum(x => x.Actual.Ticks));
        var expected = TimeSpan.FromTicks(WorkTimes.Sum(x => x.Expected.Ticks));

        if (actual > expected)
        {
            return $"Überstunden: {(actual - expected).ToTimeString()}";
        }
        else if (actual < expected)
        {
            return $"Minusstunden: {(expected - actual).ToTimeString()}";
        }

        return string.Empty;
    }

    private static string GetIconTitle(string label, TimeSpan duration, string? note)
    {
        return $"{label}: {duration.ToTimeString()}{(string.IsNullOrWhiteSpace(note) ? string.Empty : $"\n{note}")}";
    }
}
