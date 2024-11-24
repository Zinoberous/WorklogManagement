using System.Globalization;

namespace WorklogManagement.UI.Extensions;

internal static class DateTimeExtensions
{
    internal static int GetWeekOfYear(this DateOnly date)
    {
        var dateTime = date.ToDateTime(TimeOnly.MinValue);
        var calendar = CultureInfo.CurrentCulture.Calendar;
        var calendarWeekRule = CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
        var firstDayOfWeek = CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek;
        return calendar.GetWeekOfYear(dateTime, calendarWeekRule, firstDayOfWeek);
    }
}
