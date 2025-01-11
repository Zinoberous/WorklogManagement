using Microsoft.AspNetCore.Components;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public partial class TicketBoardAccordion
{
    [Parameter]
    public string Title { get; set; } = string.Empty;

    [Parameter]
    public IEnumerable<TicketGroup> TicketGroups { get; set; } = [];

    private bool IsOpen { get; set; }

    private void Toggle()
    {
        IsOpen = !IsOpen;
    }
}
