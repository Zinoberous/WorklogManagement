using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class DurationPicker
{
    [Parameter]
    public TimeOnly Value { get; set; }

    [Parameter]
    public EventCallback<TimeOnly> ValueChanged { get; set; }

    // hier wird eine Millisekunde ergänzt, damit der DatePicker von Radzen "00:00" und nicht ein leeres Input anzeigt, wenn Value == TimeOnly.MinValue
    private TimeOnly ShownValue
    {
        get => Value.Add(TimeSpan.FromMilliseconds(1));
        set
        {
            Value = value.Add(-TimeSpan.FromMilliseconds(1));
            _ = ValueChanged.InvokeAsync(Value);
        }
    }
}