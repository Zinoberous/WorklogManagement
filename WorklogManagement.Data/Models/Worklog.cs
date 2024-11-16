namespace WorklogManagement.Data.Models;

public partial class Worklog
{
    public int TimeSpentMinutes
    {
        get => TimeSpentSeconds / 60;
        set
        {
            int hours = value / 60;
            int minutes = value % 60;

            TimeSpent = new(hours, minutes, 0);

            TimeSpentSeconds = value * 60;
        }
    }
}
