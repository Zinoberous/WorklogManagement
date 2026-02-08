using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.TicketForm;

public partial class TicketFormSubTicketEnzry
{
    [Parameter]
    public required Ticket Ticket { get; set; }
}
