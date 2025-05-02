namespace WorklogManagement.UI.Extensions;

internal static class TimeSpanExtensions
{
    internal static string ToTimeString(this TimeSpan value)
    {
        return $"{(value < TimeSpan.Zero ? "-" : string.Empty)}{value:hh\\:mm}";
    }
}
