using Microsoft.AspNetCore.Components;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Shared;

public partial class AttachmentsDialog
{
    [Parameter]
    public bool IsOpen { get; set; }

    [Parameter]
    public EventCallback<bool> IsOpenChanged { get; set; }

    [Parameter]
    public IEnumerable<Attachment> Attachments { get; set; } = [];

    [Parameter]
    public EventCallback<IEnumerable<Attachment>> AttachmentsChanged { get; set; }

    [Parameter]
    public string Title { get; set; } = string.Empty;
}
