using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Column
{
    [Parameter]
    public string? ClassName { get; set; }

    private string CssClass => $"{(Fill ? "col col-fill" : "col")} {ClassName}";

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public bool Fill { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }
}