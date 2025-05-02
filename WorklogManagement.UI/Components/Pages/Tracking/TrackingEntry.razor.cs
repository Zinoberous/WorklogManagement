using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Pages.Tracking;

public partial class TrackingEntry
{
    [Parameter]
    public required Worklog Worklog { get; set; }

    [Parameter]
    public EventCallback<Worklog> OnEdit { get; set; }

    [Parameter]
    public EventCallback OnDelete { get; set; }

    [Parameter]
    public bool ShowDate { get; set; }

    private DateOnly Date
    {
        get => Worklog.Date;
        set => _ = UpdateWorklogAsync(Worklog with { Date = value });
    }

    private Ticket Ticket
    {
        get => new() { Id = Worklog.TicketId, Title = Worklog.TicketTitle };
        set => _ = UpdateWorklogAsync(Worklog with { TicketId = value.Id, TicketTitle = value.Title });
    }

    private TimeSpan TimeSpent
    {
        get => Worklog.TimeSpent;
        set => _ = UpdateWorklogAsync(Worklog with { TimeSpent = value });
    }

    private string Description
    {
        get => Worklog.Description ?? string.Empty;
        set => _ = UpdateWorklogAsync(Worklog with { Description = value });
    }

    private bool ShowDescription { get; set; }

    private void ToggleDescription() => ShowDescription = !ShowDescription;

    private IEnumerable<WorklogAttachment> Attachments => Worklog.Attachments;

    private async Task AttachmentsChanged(IEnumerable<Attachment> attachments)
    {
        var value = attachments
            .Select(x => new WorklogAttachment
            {
                Id = x.Id,
                WorklogId = Worklog.Id,
                Name = x.Name,
                Comment = x.Comment,
                Data = x.Data,
            })
            .ToArray();

        await UpdateWorklogAsync(Worklog with { Attachments = value });
    }

    private bool IsOpenAttachmentsDialog { get; set; }

    private void OpenAttachmentsDialog() => IsOpenAttachmentsDialog = true;

    private async Task UpdateWorklogAsync(Worklog worklog)
    {
        Worklog = worklog;
        await OnEdit.InvokeAsync(worklog);
    }
}
