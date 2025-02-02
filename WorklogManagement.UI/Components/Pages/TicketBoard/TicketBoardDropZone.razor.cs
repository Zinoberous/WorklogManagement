using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public partial class TicketBoardDropZone
{
    [Parameter]
    public IEnumerable<Ticket> Tickets { get; set; } = [];

    [Parameter]
    public EventCallback<Ticket> OnEdit { get; set; }

    [Parameter]
    public EventCallback<Ticket> OnDelete { get; set; }

    [Inject]
    private ITicketStatusService TicketStatusService { get; set; } = null!;

    private static bool ItemSelector(Ticket item, RadzenDropZone<Ticket> zone)
    {
        return item.Status == (TicketStatus)zone.Value;
    }

    private static void OnItemRender(RadzenDropZoneItemRenderEventArgs<Ticket> args)
    {
        args.Attributes["class"] = "rz-card rz-variant-filled";
        args.Attributes["style"] = "padding:5px; font-size:14px;";
    }

    private bool CanDrop(RadzenDropZoneItemEventArgs<Ticket> args)
    {
        return TicketStatusService.GetNextStatusOptions(args.Item.Status).Contains((TicketStatus)args.ToZone.Value);
    }

    private async Task OnDrop(RadzenDropZoneItemEventArgs<Ticket> args)
    {
        var status = (TicketStatus)args.ToZone.Value;
        var ticket = Tickets.Single(t => t.Id == args.Item.Id);

        if (ticket.Status != status)
        {
            ticket.Status = status;
            await OnEdit.InvokeAsync(ticket);
        }
    }
}
