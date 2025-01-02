using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class FlexRow
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"flex flex-row {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    private string CssStyle => $"flex-wrap: {(Wrap ? "wrap" : "nowrap")}; {Style}";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Wrap { get; set; }
}
