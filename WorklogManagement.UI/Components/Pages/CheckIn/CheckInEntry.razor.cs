using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInEntry
{
    [Parameter]
    public IEnumerable<string> TypeOptions { get; set; } = [];

    [Parameter]
    public string? SelectedType { get; set; } 

    [Parameter]
    public EventCallback<string> SelectedTypeChanged { get; set; }

    [Parameter]
    public TimeSpan Actual { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ActualChanged { get; set; }

    private TimeOnly ActualValue
    {
        get => TimeOnly.FromTimeSpan(Actual);
        set
        {
            Actual = value.ToTimeSpan();
            ActualChanged.InvokeAsync(Actual);
        }
    }

    [Parameter]
    public TimeSpan? Expected { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ExpectedChanged { get; set; }

    private TimeOnly ExpectedValue
    {
        get => TimeOnly.FromTimeSpan(Expected ?? TimeSpan.Zero);
        set
        {
            Expected = value.ToTimeSpan();
            ExpectedChanged.InvokeAsync(Expected.Value);
        }
    }

    [Parameter]
    public string? Note { get; set; }

    [Parameter]
    public EventCallback<string?> NoteChanged { get; set; }

    [Parameter]
    public EventCallback OnDelte { get; set; }
}
