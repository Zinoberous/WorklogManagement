using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace WorklogManagement.UI.Components.Shared;

public partial class Accordion
{
    [Parameter]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Parameter]
    public string? Class { get; set; }

    private string ClassValue => $"accordion {AccordionClass} {Class}".Trim();

    private string _accordionClass = string.Empty;
    private string AccordionClass
    {
        get => _accordionClass;
        set
        {
            _accordionClass = value;
            StateHasChanged();
        }
    }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool IsOpen { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        if (IsOpen)
        {
            AccordionClass = "accordion-expanded";
        }
        else
        {
            AccordionClass = "accordion-collapsed";
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        await base.OnParametersSetAsync();

        await CheckIsOpenSetAsync();
    }

    private bool? _lastIsOpen;
    private async Task CheckIsOpenSetAsync()
    {
        if (_lastIsOpen == IsOpen)
        {
            return;
        }

        var firstRender = _lastIsOpen is null;

        _lastIsOpen = IsOpen;

        if (firstRender)
        {
            return;
        }

        await ToggleAsync();
    }

    private async Task ToggleAsync()
    {
        if (IsOpen)
        {
            AccordionClass = "accordion-expanding";

            await JSRuntime.InvokeVoidAsync("slideDown", Id, 500);

            AccordionClass = "accordion-expanded";
        }
        else
        {
            AccordionClass = "accordion-collapsing";

            await JSRuntime.InvokeVoidAsync("slideUp", Id, 500);

            AccordionClass = "accordion-collapsed";
        }
    }
}
