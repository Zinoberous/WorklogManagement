using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Grid
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"grid {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}