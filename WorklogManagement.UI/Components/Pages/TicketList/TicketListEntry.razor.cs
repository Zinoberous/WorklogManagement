using Microsoft.AspNetCore.Components;
using Radzen;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Common;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketList;
public partial class TicketListEntry
{
    [Inject]
    public required ITicketStatusService TicketStatusService { get; set; }

    [Parameter]
    public required Ticket Ticket { get; set; }

    [Parameter]
    public EventCallback<Ticket> OnEdit { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }

    private string Title
    {
        get => Ticket.Title;
        set => _ = UpdateTicketAsync(Ticket with { Title = value });
    }

    private string Description
    {
        get => Ticket.Description ?? string.Empty;
        set => _ = UpdateTicketAsync(Ticket with { Description = string.IsNullOrEmpty(value) ? null : value });
    }

    private bool ShowDescription { get; set; }

    private TicketStatus SelectedStatus
    {
        get => Ticket.Status;
        set
        {
            _ = UpdateTicketAsync(Ticket with { Status = value });
        }
    }

    private IReadOnlyDictionary<TicketStatus, string> TicketStatusChangeOptions
        => TicketStatusService.GetNextStatusOptions(Ticket.Status).ToDictionary(x => x, x => x.ToString());

    private string StatusStyle => $"color: {Constant.TicketStatusColor[SelectedStatus]}; background-color: {Constant.TicketStatusBgColor[SelectedStatus]}";

    private void StatusRender(DropDownItemRenderEventArgs<TicketStatus> args)
    {
        if (args.Item is KeyValuePair<TicketStatus, string> item)
        {
            var status = item.Key;

            if (status == SelectedStatus)
            {
                args.Attributes.Add("style", "display: none;");
            }
            else
            {
                args.Attributes.Add("style", $"color: {Constant.TicketStatusColor[status]}; background-color: {Constant.TicketStatusBgColor[status]};");
            }
        }
    }

    private string StatusNote
    {
        get => Ticket.StatusNote ?? string.Empty;
        set => _ = UpdateTicketAsync(Ticket with { StatusNote = string.IsNullOrEmpty(value) ? null : value });
    }

    private bool ShowStatusNote { get; set; }

    private async Task UpdateTicketAsync(Ticket ticket)
    {
        Ticket = ticket;
        await OnEdit.InvokeAsync(ticket);
    }
}
