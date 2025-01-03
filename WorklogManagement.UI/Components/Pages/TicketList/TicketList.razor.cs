using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.TicketList;

public partial class TicketList
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "status")]
    public string? InitialStatusFilter { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "search")]
    public string? InitialSearch { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitAsync(InitialStatusFilter, InitialSearch);
    }
}
