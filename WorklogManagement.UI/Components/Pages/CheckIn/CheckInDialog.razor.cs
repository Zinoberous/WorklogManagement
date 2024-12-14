using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.CheckIn;

public partial class CheckInDialog
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public DateOnly Date { get; set; }

    [Parameter]
    public EventCallback<WorkTime> OnSaveWorkTime { get; set; }

    [Parameter]
    public EventCallback<Absence> OnSaveAbsence { get; set; }

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);
    }
}
