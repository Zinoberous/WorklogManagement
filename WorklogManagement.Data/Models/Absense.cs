namespace WorklogManagement.Data.Models;

public partial class Absence
{
    public TimeSpan Duration
    {
        get => TimeSpan.FromSeconds(DurationSeconds);
        set => DurationSeconds = (int)value.TotalSeconds;
    }
}
