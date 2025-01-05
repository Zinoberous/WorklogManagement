using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Reflection.Metadata;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Shared;

public partial class AttachmentPanel
{
    [Parameter]
    public IEnumerable<Attachment> Attachments { get; set; } = [];

    [Parameter]
    public EventCallback<IEnumerable<Attachment>> AttachmentsChanged { get; set; }

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = null!;

    private async Task Add(UploadChangeEventArgs args)
    {
        ICollection<Attachment> attachments = [];

        foreach (var file in args.Files)
        {
            using var stream = file.OpenReadStream();

            using MemoryStream memoryStream = new();
            await stream.CopyToAsync(memoryStream);

            attachments.Add(new Attachment
            {
                Name = file.Name,
                Data = Convert.ToBase64String(memoryStream.ToArray())
            });
        }

        Attachments = attachments;
        await AttachmentsChanged.InvokeAsync(Attachments);
    }

    private async Task Edit(Attachment attachment)
    {
        Attachments = Attachments.Select(old => old.Name == attachment.Name ? attachment : old).ToArray();
        await AttachmentsChanged.InvokeAsync(Attachments);
    }

    private async Task Remove(Attachment attachment)
    {
        Attachments = Attachments.Where(a => a.Name != attachment.Name).ToArray();
        await AttachmentsChanged.InvokeAsync(Attachments);
    }

    private async Task Download(Attachment attachment)
    {
        await JSRuntime.InvokeVoidAsync("downloadFile", attachment.Name, attachment.Data);
    }
}
