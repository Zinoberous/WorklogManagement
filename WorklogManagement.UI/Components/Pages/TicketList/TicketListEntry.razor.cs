using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.TicketList;
public partial class TicketListEntry
{
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
        set => _ = UpdateTicketAsync(Ticket with { Description = value });
    }

    private bool ShowDescription { get; set; }

    private void ToggleDescription() => ShowDescription = !ShowDescription;

    private IEnumerable<TicketAttachment> Attachments => Ticket.Attachments;

    private async Task AttachmentsChanged(IEnumerable<Attachment> attachments)
    {
        var value = attachments
            .Select(x => new TicketAttachment
            {
                Id = x.Id,
                TicketId = Ticket.Id,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
            })
            .ToArray();

        await UpdateTicketAsync(Ticket with { Attachments = value });
    }

    private bool IsOpenAttachmentsDialog { get; set; }

    private void OpenAttachmentsDialog() => IsOpenAttachmentsDialog = true;

    private TicketStatus SelectedStatus
    {
        get => Ticket.Status;
        set
        {
            _ = UpdateTicketAsync(Ticket with { Status = value, StatusStartedAt = DateTime.UtcNow });
        }
    }

    private string StatusNote
    {
        get => Ticket.StatusNote ?? string.Empty;
        set => _ = UpdateTicketAsync(Ticket with { StatusNote = value });
    }

    private bool ShowStatusNote { get; set; }

    private void ToggleStatusNote() => ShowStatusNote = !ShowStatusNote;

    private async Task UpdateTicketAsync(Ticket ticket)
    {
        Ticket = ticket;
        await OnEdit.InvokeAsync(ticket);
    }
}
