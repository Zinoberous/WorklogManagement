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
    public required RenderFragment ChildContent { get; set; }

    private async Task Close()
    {
        IsOpen = false;

        await IsOpenChanged.InvokeAsync(IsOpen);
    }
}
