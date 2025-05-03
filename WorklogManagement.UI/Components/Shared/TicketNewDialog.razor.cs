using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Shared;

public partial class TicketNewDialog
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public Func<Ticket, Task<bool>> OnSave { get; set; } = _ => Task.FromResult(false);

    private string Title { get; set; } = string.Empty;

    private TicketStatus Status { get; set; } = TicketStatus.Todo;

    private RefTicket? Ref { get; set; }

    private Ticket? RefTicketSearch
    {
        get => Ref is not null ? new Ticket { Id = Ref.Id, Title = Ref.Title } : null;
        set => Ref = value is not null ? new RefTicket { Id = value.Id, Title = value.Title } : null;
    }

    private string? Description { get; set; }

    private IEnumerable<TicketAttachment> Attachments { get; set; } = [];

    private void AttachmentsChanged(IEnumerable<Attachment> attachments)
    {
        Attachments = [
            .. attachments.Select(x => new TicketAttachment
            {
                Id = x.Id,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
            })
        ];
    }

    private async Task Save()
    {
        Ticket ticket = new()
        {
            Ref = Ref,
            Title = Title,
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
            Status = Status,
            Attachments = Attachments
        };
        await OnSave.Invoke(ticket);
        await Close();
    }

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);

        Reset();
    }

    private void Reset()
    {
        Title = string.Empty;
        Status = TicketStatus.Todo;
        Ref = null;
        Description = null;
        Attachments = [];
    }
}
