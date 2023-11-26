namespace WorklogManagement.API.Helper
{
    internal static class TimeHelper
    {
        internal static int TimeToSeconds(TimeOnly time)
        {
            return (time.Hour * 3600) + (time.Minute * 60) + time.Second;
        }

        internal static TimeOnly SecondsToTime(int seconds)
        {
            int hours = seconds / 3600;
            int minutes = (seconds % 3600) / 60;
            int remainingSeconds = seconds % 60;

            return new TimeOnly(hours, minutes, remainingSeconds);
        }
    }
}
