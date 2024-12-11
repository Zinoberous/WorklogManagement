using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class DurationPicker
{
    [Parameter]
    public TimeOnly Value { get; set; }

    [Parameter]
    public EventCallback<TimeOnly> ValueChanged { get; set; }

    // hier wird eine Millisekunde ergänzt,
    // damit der DatePicker von Radzen "00:00" und nicht ein leeres Input anzeigt,
    // wenn Value == TimeOnly.MinValue
    private TimeOnly ShownValue
    {
        get => Value == TimeOnly.MinValue ? Value.Add(TimeSpan.FromMilliseconds(1)) : Value;
        set => _ = SetValue(value);
    }

    private async Task SetValue(TimeOnly value)
    {
        Value = value.Millisecond != 0 ? value.Add(-TimeSpan.FromMilliseconds(1)) : value;
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task Increment()
    {
        if (Value == TimeOnly.MaxValue)
        {
            return;
        }

        // 1_425 = 23:45
        if (ShownValue.ToTimeSpan().TotalMinutes > 1_425)
        {
           await  SetValue(TimeOnly.MaxValue);
        }
        else
        {
            await SetValue(Value.Add(TimeSpan.FromMinutes(15)));
        }
    }

    private async Task Decrement()
    {
        if (Value == TimeOnly.MinValue)
        {
            return;
        }

        if (ShownValue.ToTimeSpan().TotalMinutes < 15)
        {
            await SetValue(TimeOnly.MinValue);
        }
        else
        {
            await SetValue(Value.Add(-TimeSpan.FromMinutes(15)));
        }
    }
}