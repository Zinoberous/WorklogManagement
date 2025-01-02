using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class FlexCol
{
    [Parameter]
    public string? Class { get; set; }

    private string ClassValue => $"flex flex-col {Class}".Trim();

    [Parameter]
    public string? Style { get; set; }

    private string StyleValue => $"{Style}";

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}
