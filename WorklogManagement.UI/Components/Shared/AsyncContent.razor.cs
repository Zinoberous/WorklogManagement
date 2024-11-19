using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Shared;

public partial class AsyncContent
{
    [Parameter]
    public required bool IsLoading { get; set; }

    [Parameter]
    public string? Error { get; set; }

    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
