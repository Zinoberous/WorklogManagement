using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Enums;

namespace WorklogManagement.UI.Components.Shared;

public partial class TicketStatusEntry
{
    [Parameter]
    public required TicketStatus Status { get; set; }
}
