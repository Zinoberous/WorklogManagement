using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Row
{
    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
