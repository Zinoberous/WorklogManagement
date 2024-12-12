using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Dialog
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    private async Task Close()
    {
        await IsOpenChanged.InvokeAsync(false);
    }
}
