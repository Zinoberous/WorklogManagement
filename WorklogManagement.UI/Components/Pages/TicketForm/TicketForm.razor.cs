using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.TicketForm;

public partial class TicketForm
{
    [Parameter]
    public required int Id { get; set; }

    protected override async Task OnParametersSetAsync()
    {
        await ViewModel.LoadAsync(Id);
    }
}
