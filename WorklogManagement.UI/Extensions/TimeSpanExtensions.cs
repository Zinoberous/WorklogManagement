namespace WorklogManagement.UI.Extensions;

internal static class TimeSpanExtensions
{
    internal static string ToTimeString(this TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm");
    }
}
