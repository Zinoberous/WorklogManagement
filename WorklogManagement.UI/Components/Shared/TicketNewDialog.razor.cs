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

    private RefTicket? Ref { get; set; }

    private string? Description { get; set; }

    private IEnumerable<TicketAttachment> Attachments { get; set; } = [];

    private void AttachmentsChanged(IEnumerable<Attachment> attachments)
    {
        Attachments = attachments
            .Select(x => new TicketAttachment
            {
                Id = x.Id,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
            })
            .ToArray();
    }

    private async Task Close()
    {
        IsOpen = false;
        await IsOpenChanged.InvokeAsync(IsOpen);

        Title = string.Empty;
        Ref = null;
        Description = null;
        Attachments = [];
    }

    private async Task Save()
    {
        Ticket ticket = new()
        {
            Ref = Ref,
            Title = Title,
            Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
            Status = TicketStatus.Todo,
            Attachments = Attachments
        };
        await OnSave.Invoke(ticket);
        await Close();
    }
}