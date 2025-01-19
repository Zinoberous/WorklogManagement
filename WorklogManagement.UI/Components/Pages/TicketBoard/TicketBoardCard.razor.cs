using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;
using WorklogManagement.UI.Services;

namespace WorklogManagement.UI.Components.Pages.TicketBoard;

public partial class TicketBoardCard
{
    [Parameter]
    public Ticket Ticket { get; set; } = null!;

    [Parameter]
    public EventCallback<Ticket> OnEdit { get; set; }

    [Parameter]
    public EventCallback<Ticket> OnDelete { get; set; }

    [Inject]
    private INavigationService NavigationService { get; set; } = null!;

    private bool IsOpenStatusNoteDialog { get; set; }

    private void OpenStatusNoteDialog()
    {
        IsOpenStatusNoteDialog = true;
    }

    private async Task SaveStatusNote(string value)
    {
        Ticket.StatusNote = value;
        await OnEdit.InvokeAsync(Ticket);
    }

    private void NavigateToTicket()
    {
        NavigationService.NavigateToPage($"/tickets/{Ticket.Id}");
    }
}
