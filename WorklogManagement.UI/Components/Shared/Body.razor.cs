using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Body
{
    [Parameter]
    public string? Class { get; set; }

    private string ClassValue => $"body {Class}".Trim();

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
