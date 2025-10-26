namespace WorklogManagement.UI.Extensions;

internal static class TimeSpanExtensions
{
    internal static string ToTimeString(this TimeSpan value)
    {
        var sign = value < TimeSpan.Zero ? "-" : string.Empty;
        var abs = value.Duration();

        // Absolute Stunden und Minuten (keine 24h-Rotation)
        var totalHours = abs.Ticks / TimeSpan.TicksPerHour;
        var minutes = (int)((abs.Ticks % TimeSpan.TicksPerHour) / TimeSpan.TicksPerMinute);

        return $"{sign}{totalHours}:{minutes:00}";
    }
}
