using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public partial class TicketBoardDropZone
{
    [Parameter]
    public IEnumerable<Ticket> Tickets { get; set; } = [];

    private static bool ItemSelector(Ticket item, RadzenDropZone<Ticket> zone)
    {
        return item.Status == (TicketStatus)zone.Value;
    }

    private static void OnItemRender(RadzenDropZoneItemRenderEventArgs<Ticket> args)
    {
        args.Attributes["class"] = "rz-card rz-variant-filled rz-background-color-primary-darker rz-color-on-primary-dark";
    }
}
