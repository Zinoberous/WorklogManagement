using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace WorklogManagement.UI.Components.Shared;

public partial class AsyncContent
{
    [Parameter]
    public required bool IsLoading { get; set; }

    [Parameter]
    public Exception? Error { get; set; }

    [Parameter]
    public required RenderFragment ChildContent { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    [Inject]
    private NotificationService NotificationService { get; set; } = null!;

    protected override async Task OnParametersSetAsync()
    {
        if (Error is not null)
        {
            NotificationService.Notify(NotificationSeverity.Error, Error.Message);

            await JSRuntime.InvokeVoidAsync("console.error", Error.ToString());
        }

        await base.OnParametersSetAsync();
    }
}
