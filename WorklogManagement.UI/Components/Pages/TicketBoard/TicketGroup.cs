using OneOf;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public record TicketGroup
{
    public required OneOf<Ticket, RefTicket> Parent { get; init; }

    public IEnumerable<TicketGroup> Childs { get; init; } = [];
}
