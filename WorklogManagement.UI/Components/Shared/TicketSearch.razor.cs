using Microsoft.AspNetCore.Components;
using Radzen;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Enums;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Shared;

public partial class TicketSearch
{
    [Parameter]
    public Ticket? Value { get; set; }

    [Parameter]
    public EventCallback<Ticket?> ValueChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    [Inject]
    private IDataService DataService { get; set; } = null!;

    private IEnumerable<Ticket> Tickets { get; set; } = [];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (firstRender)
        {
            await Search(null);
        }
    }

    private async Task LoadData(LoadDataArgs args) => await Search(args.Filter);
    private async Task Search(string? searchText)
    {
        Tickets = string.IsNullOrWhiteSpace(searchText)
            ? (await DataService.GetTicketsPageByStatusFilterAsync(0, 0, [TicketStatus.Running, TicketStatus.Continuous])).Items
            : (await DataService.GetTicketsPageBySearchAsync(0, 0, searchText, TicketSearchType.Title)).Items;

        await InvokeAsync(StateHasChanged);
    }


}
