using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class FlexFill
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"flex-fill {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    private string CssStyle => $"{Style}";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
