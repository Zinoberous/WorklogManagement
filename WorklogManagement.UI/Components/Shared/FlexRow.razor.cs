using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class FlexRow
{
    [Parameter]
    public string? Class { get; set; }

    private string ClassValue => $"flex flex-row{(Fill ? "-fill" : string.Empty)} {Class}".Trim();

    [Parameter]
    public string? Style { get; set; }

    private string StyleValue => $"flex-wrap: {(Wrap ? "wrap" : "nowrap")}; {Style}";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Fill { get; set; }

    [Parameter]
    public bool Wrap { get; set; }
}
