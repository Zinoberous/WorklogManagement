using Microsoft.AspNetCore.Components;
using Radzen;

namespace WorklogManagement.UI.Components.Shared;

public partial class DatePicker
{
    [Parameter]
    public DateOnly Value { get; set; }

    [Parameter]
    public EventCallback<DateOnly> ValueChanged { get; set; }

    [Parameter]
    public EventCallback<DateOnly> OnChanged { get; set; }

    [Parameter]
    public Action<DateRenderEventArgs> DateRender { get; set; } = _ => { };

    private DateOnly Date
    {
        get => Value;
        set
        {
            Value = value;
            ValueChanged.InvokeAsync(Value);
            OnChanged.InvokeAsync(Value);
        }
    }
}