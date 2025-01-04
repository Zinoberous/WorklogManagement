using Microsoft.AspNetCore.Components;
using Radzen;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Enums;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Shared;

public partial class TicketSearch
{
    [Parameter]
    public RefTicket? Value { get; set; }

    [Parameter]
    public EventCallback<RefTicket?> ValueChanged { get; set; }

    [Parameter]
    public string Placeholder { get; set; } = string.Empty;

    [Inject]
    private IDataService DataService { get; set; } = null!;

    private IEnumerable<RefTicket> Tickets { get; set; } = [];

    private async Task Search(LoadDataArgs args)
    {
        var searchText = args.Filter;

        if (string.IsNullOrWhiteSpace(searchText))
        {
            return;
        }

        Tickets = (await DataService.GetTicketsPageBySearchAsync(0, 0, searchText, TicketSearchType.Title))
            .Items.Select(x => new RefTicket { Id = x.Id, Title = x.Title })
            .ToArray();

        await InvokeAsync(StateHasChanged);
    }
}
