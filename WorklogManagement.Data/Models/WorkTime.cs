namespace WorklogManagement.Data.Models;

public partial class WorkTime
{
    //public TimeOnly Expected
    //{
    //    get => new(ExpectedMinutes / 60, ExpectedMinutes % 60);
    //    set => ExpectedMinutes = value.Hour * 60 + value.Minute;
    //}

    //public int ExpectedSeconds
    //{
    //    get => ExpectedMinutes * 60;
    //    set => ExpectedMinutes = value / 60;
    //}

    public TimeSpan ExpectedSpan
    {
        get => TimeSpan.FromMinutes(ExpectedMinutes);
        set => ExpectedMinutes = (int)value.TotalMinutes;
    }

    //public TimeOnly Actual
    //{
    //    get => new(ActualMinutes / 60, ActualMinutes % 60);
    //    set => ActualMinutes = value.Hour * 60 + value.Minute;
    //}

    //public int ActualSeconds
    //{
    //    get => ActualMinutes * 60;
    //    set => ActualMinutes = value / 60;
    //}

    public TimeSpan ActualSpan
    {
        get => TimeSpan.FromMinutes(ActualMinutes);
        set => ActualMinutes = (int)value.TotalMinutes;
    }
}
