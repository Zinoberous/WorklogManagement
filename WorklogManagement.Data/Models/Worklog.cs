namespace WorklogManagement.Data.Models;

public partial class Worklog
{
    public TimeSpan TimeSpent
    {
        get => TimeSpan.FromSeconds(TimeSpentSeconds);
        set => TimeSpentSeconds = (int)value.TotalSeconds;
    }
}
