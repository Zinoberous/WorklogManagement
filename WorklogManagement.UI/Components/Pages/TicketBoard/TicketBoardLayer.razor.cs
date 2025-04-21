using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public partial class TicketBoardLayer
{
    [Parameter]
    public IEnumerable<TicketGroup> TicketGroups { get; set; } = [];

    [Parameter]
    public EventCallback<Ticket> OnEdit { get; set; }

    [Parameter]
    public EventCallback<Ticket> OnDelete { get; set; }

    // Parent kann nur RefTicket sein, wenn es durch ein Ticket (Child) angegeben wurde und das zugehörige Ticket nicht in der Liste der Tickets enthalten ist
    private IEnumerable<Ticket> LayerTickets => [
        .. TicketGroups
            .Where(x => !x.Childs.Any())
            .Select(x => x.Parent.AsT0)
            .OrderBy(x => x.Title)
    ];

    private Dictionary<string, IEnumerable<TicketGroup>> SubLayer => TicketGroups
        .Where(x => x.Childs.Any())
        .OrderBy(x => x.Parent.IsT0 ? x.Parent.AsT0.Title : x.Parent.AsT1.Title)
        .ToDictionary(
            x => x.Parent.IsT0
                ? x.Parent.AsT0.Title
                : x.Parent.AsT1.Title,
            x => x.Parent.IsT0 && x.Childs.Any()
                ? [new() { Parent = x.Parent.AsT0 }, .. x.Childs]
                : x.Childs
        );
}
