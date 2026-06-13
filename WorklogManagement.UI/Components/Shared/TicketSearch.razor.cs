using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components;
using Radzen;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Common;
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

    [Parameter]
    public bool AllowClear { get; set; }

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
        var searchResult = string.IsNullOrWhiteSpace(searchText)
            ? await DataService.GetTicketsAsync(0, 0, $"id == 1 or status == {(int)TicketStatus.Running}")
            : await DataService.GetTicketsAsync(0, 0, $@"Title.Contains(""{searchText}"")");

        Tickets = searchResult.Items
            .OrderBy(x => Constants.TicketStatusDone.Contains(x.Status) ? 1 : 0)
            .ThenBy(x => x.Id);

        await InvokeAsync(StateHasChanged);
    }
}
