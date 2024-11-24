using Microsoft.AspNetCore.Components;
using WorklogManagement.UI.Enums;

namespace WorklogManagement.UI.Components.Shared;

public partial class AsyncContent
{
    [Parameter]
    public required bool IsLoading { get; set; }

    [Parameter]
    public AsyncContentRenderMode RenderMode { get; set; } = AsyncContentRenderMode.AfterLoading;

    [Parameter]
    public required RenderFragment ChildContent { get; set; }
}
