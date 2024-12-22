using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using WorklogManagement.Shared.Enums;

namespace WorklogManagement.UI.Components.Pages.TicketList;

public partial class TicketList
{
    [Inject]
    public required IJSRuntime JSRuntime { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "status")]
    public string? InitialStatusFilter { get; set; }

    [Parameter]
    [SupplyParameterFromQuery(Name = "search")]
    public string? InitialSearch { get; set; }

    protected override async Task OnInitializedAsync()
    {
        string? statusFilter = InitialStatusFilter;

        if (string.IsNullOrEmpty(statusFilter))
        {
            statusFilter = await JSRuntime.InvokeAsync<string?>("localStorage.getItem", "ticket-list.status");
        }

        await ViewModel.InitAsync(statusFilter?.Split(',').Select(Enum.Parse<TicketStatus>), InitialSearch);
    }
}