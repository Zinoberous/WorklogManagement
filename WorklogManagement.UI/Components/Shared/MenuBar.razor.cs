using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class MenuBar
{
    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
