using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class DurationPicker
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"duration-picker {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public TimeSpan Value { get; set; }

    [Parameter]
    public EventCallback<TimeSpan> ValueChanged { get; set; }

    private static TimeSpan MinValue => TimeSpan.Zero;
    private static TimeSpan MaxValue => new TimeOnly(23, 59).ToTimeSpan();
    private static TimeSpan MinutesStep => TimeSpan.FromMinutes(15);

    private TimeOnly TimeValue
    {
        get
        {
            if (Value == MinValue)
            {
                // hier wird eine Millisekunde ergänzt, damit der DatePicker von Radzen "00:00" und nicht ein leeres Input anzeigt
                return TimeOnly.MinValue.Add(TimeSpan.FromMilliseconds(1));
            }

            // DurationPicker erlaubt nur Stunden und Minuten im Bereich von 00:00 bis 23:59
            return new(Value.Hours, Value.Minutes);
        }
        set
        {
            var newValue = value.ToTimeSpan();

            if (newValue.Milliseconds == 1)
            {
                // hier wird die hinzugefügte Millisekunde wieder entfernt
                newValue = newValue.Add(-TimeSpan.FromMilliseconds(1));
            }

            _ = SetValue(newValue);
        }
    }

    private async Task SetValue(TimeSpan value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task Increment()
    {
        if (Value == MaxValue)
        {
            return;
        }

        // 1_425 = 23:45
        if (Value.TotalMinutes > 1_425)
        {
           await  SetValue(MaxValue);
        }
        else
        {
            await SetValue(Value.Add(MinutesStep));
        }
    }

    private async Task Decrement()
    {
        if (Value == MinValue)
        {
            return;
        }

        if (Value.TotalMinutes < 15)
        {
            await SetValue(MinValue);
        }
        else
        {
            await SetValue(Value.Add(-MinutesStep));
        }
    }
}