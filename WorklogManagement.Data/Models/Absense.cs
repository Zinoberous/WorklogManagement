namespace WorklogManagement.Data.Models;

public partial class Absence
{
    //public TimeOnly Duration
    //{
    //    get => new(DurationMinutes / 60, DurationMinutes % 60);
    //    set => DurationMinutes = value.Hour * 60 + value.Minute;
    //}

    //public int DurationSeconds
    //{
    //    get => DurationMinutes * 60;
    //    set => DurationMinutes = value / 60;
    //}

    public TimeSpan DurationSpan
    {
        get => TimeSpan.FromMinutes(DurationMinutes);
        set => DurationMinutes = (int)value.TotalMinutes;
    }
}
