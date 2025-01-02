using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class FlexCol
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"flex flex-col {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    private string CssStyle => $"{Style}";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
