using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInEntry
{
    [Parameter]
    public string SelectedType { get; set; } = null!;

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
    public EventCallback<TimeOnly>? ExpectedChanged { get; set; }

    [Parameter]
    public string? Note { get; set; }

    [Parameter]
    public EventCallback<string?> NoteChanged { get; set; }

    [Parameter]
    public EventCallback OnDelte { get; set; }

    private DateTime ActualValue
    {
        get => new(new(), Actual);
        set
        {
            Actual = TimeOnly.FromDateTime(value);
            _ = ActualChanged.InvokeAsync(Actual);
        }
    }

    private DateTime? ExpectedValue
    {
        get => Expected.HasValue ? new(new(), Expected.Value) : null;
        set
        {
            Expected = TimeOnly.FromDateTime(value!.Value);
            _ = ExpectedChanged?.InvokeAsync(Expected!.Value);
        }
    }
}
