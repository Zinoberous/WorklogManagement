using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.Ticket;

public partial class Ticket
{
    [Parameter]
    public required int Id { get; set; }
}