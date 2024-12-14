using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInDurationPicker
{
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
}
