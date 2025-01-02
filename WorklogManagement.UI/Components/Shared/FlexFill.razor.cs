using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class FlexFill
{
    [Parameter]
    public string? Class { get; set; }

    private string ClassValue => $"flex-fill {Class}".Trim();

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
