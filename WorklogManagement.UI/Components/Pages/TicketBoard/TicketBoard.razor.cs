using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public partial class TicketBoard
{
    [Parameter]
    [SupplyParameterFromQuery(Name = "search")]
    public string? InitialSearch { get; set; }

    private Dictionary<int, Ticket> TicketDict => ViewModel.Tickets.ToDictionary(x => x.Id);

    private IEnumerable<TicketGroup> TicketGroups => GetTicketGroups();

    protected override async Task OnInitializedAsync()
    {
        await ViewModel.InitAsync(InitialSearch);
    }

    private TicketGroup[] GetTicketGroups(int? ticketId = null)
    {
        if (ticketId is null)
        {
            return [
                .. ViewModel.Tickets
                    .Where(x => x.Ref is null)
                    .Select(x => new TicketGroup { Parent = x, Childs = GetTicketGroups(x.Id) })
                    .ToArray(),
                .. ViewModel.Tickets
                    .Where(x => x.Ref is not null && !TicketDict.ContainsKey(x.Ref.Id))
                    .Select(x => new TicketGroup { Parent = x.Ref!, Childs = GetTicketGroups(x.Ref!.Id) })
                    .ToArray()
            ];
        }

        return ViewModel.Tickets
            .Where(x => x.Ref is not null && x.Ref.Id == ticketId)
            .Select(x => new TicketGroup { Parent = x, Childs = GetTicketGroups(x.Id) })
            .ToArray();
    }
}
