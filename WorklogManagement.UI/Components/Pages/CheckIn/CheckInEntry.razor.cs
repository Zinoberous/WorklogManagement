using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInEntry
{
    [Parameter]
    public string? Type { get; set; }

    [Parameter]
    public TimeSpan Actual { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ActualChanged { get; set; }

    private TimeSpan ActualValue
    {
        get => Actual;
        set
        {
            Actual = value;
            ActualChanged.InvokeAsync(Actual);
        }
    }

    [Parameter]
    public TimeSpan? Expected { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ExpectedChanged { get; set; }

    private TimeSpan ExpectedValue
    {
        get => Expected ?? TimeSpan.Zero;
        set
        {
            Expected = value;
            ExpectedChanged.InvokeAsync(Expected.Value);
        }
    }

    [Parameter]
    public string? Note { get; set; }

    [Parameter]
    public EventCallback<string?> NoteChanged { get; set; }

    private string? NoteValue
    {
        get => Note;
        set
        {
            Note = value;
            NoteChanged.InvokeAsync(Note);
        }
    }

    [Parameter]
    public EventCallback OnDelte { get; set; }
}
