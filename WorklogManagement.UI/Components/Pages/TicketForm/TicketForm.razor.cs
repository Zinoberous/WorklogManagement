using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.TicketForm;

public partial class TicketForm
{
    [Parameter]
    public required int Id { get; set; }

    private string TimeSpent => $"Arbeitsprotokoll ({$"{(int)ViewModel.TimeSpent.TotalHours}".PadLeft(2, '0')}:{$"{ViewModel.TimeSpent.Minutes}".PadLeft(2, '0')})";

    protected override async Task OnParametersSetAsync()
    {
        await ViewModel.LoadAsync(Id);
    }
}
