using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInEntry
{
    [Parameter]
    public string? SelectedType { get; set; } 

    [Parameter]
    public EventCallback<string> SelectedTypeChanged { get; set; }

    [Parameter]
    public IEnumerable<string> TypeOptions { get; set; } = [];

    [Parameter]
    public TimeOnly Actual { get; set; }

    [Parameter]
    public EventCallback<TimeOnly> ActualChanged { get; set; }

    [Parameter]
    public TimeOnly? Expected { get; set; }

    [Parameter]
    public EventCallback<TimeOnly> ExpectedChanged { get; set; }

    private TimeOnly ExpectedValue
    {
        get => Expected ?? TimeOnly.MinValue;
        set
        {
            Expected = value;
            _ = ExpectedChanged.InvokeAsync(Expected!.Value);
        }
    }

    [Parameter]
    public string? Note { get; set; }

    [Parameter]
    public EventCallback<string?> NoteChanged { get; set; }

    [Parameter]
    public EventCallback OnDelte { get; set; }
}
