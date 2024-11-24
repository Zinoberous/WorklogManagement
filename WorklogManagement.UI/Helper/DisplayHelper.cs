namespace WorklogManagement.UI.Helper;

internal static class DisplayHelper
{
    internal static string MinutesToTime(int totalMinutes)
    {
        int hours = totalMinutes / 60;
        int minutes = totalMinutes % 60;

        return $"{hours:D2}:{minutes:D2}";
    }
}
