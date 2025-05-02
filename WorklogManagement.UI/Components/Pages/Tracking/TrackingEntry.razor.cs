using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.Tracking;

public partial class TrackingEntry
{
    [Parameter]
    public required Worklog Worklog { get; set; }

    [Parameter]
    public EventCallback<Worklog> OnEdit { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }
}
