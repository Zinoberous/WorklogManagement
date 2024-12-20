namespace WorklogManagement.Data.Models;

public partial class WorkTime
{
    public TimeSpan Expected
    {
        get => TimeSpan.FromSeconds(ExpectedSeconds);
        set => ExpectedSeconds = (int)value.TotalSeconds;
    }

    public TimeSpan Actual
    {
        get => TimeSpan.FromSeconds(ActualSeconds);
        set => ActualSeconds = (int)value.TotalSeconds;
    }
}
