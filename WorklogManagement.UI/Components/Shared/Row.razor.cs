using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Row
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"row {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
