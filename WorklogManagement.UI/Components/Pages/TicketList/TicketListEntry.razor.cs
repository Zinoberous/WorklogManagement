using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketList;
public partial class TicketListEntry
{
    [Parameter]
    public required Ticket Ticket { get; set; }

    [Parameter]
    public EventCallback<Ticket> OnEdit { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }

    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

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

    private void ToggleDescription() => ShowDescription = !ShowDescription;

    private TicketStatus SelectedStatus
    {
        get => Ticket.Status;
        set
        {
            _ = UpdateTicketAsync(Ticket with { Status = value });
        }
    }

    private string StatusNote
    {
        get => Ticket.StatusNote ?? string.Empty;
        set => _ = UpdateTicketAsync(Ticket with { StatusNote = string.IsNullOrEmpty(value) ? null : value });
    }

    private bool ShowStatusNote { get; set; }

    private void ToggleStatusNote() => ShowStatusNote = !ShowStatusNote;

    private async Task UpdateTicketAsync(Ticket ticket)
    {
        Ticket = ticket;
        await OnEdit.InvokeAsync(ticket);
    }

    private void NavigateToTicket()
    {
        // TODO: NavigationService.NavigateToPage($"/tickets/{Ticket.Id}");
        NavigationService.NavigateToPage("/");
    }
}
