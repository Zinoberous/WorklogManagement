using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class Col
{
    [Parameter]
    public bool Fill { get; set; }

    [Parameter]
    public string? Style { get; set; }

    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    private string CssClass => Fill ? "col col-fill" : "col";
}
