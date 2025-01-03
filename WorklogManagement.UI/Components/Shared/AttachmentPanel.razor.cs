using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;
using System.Reflection.Metadata;
using WorklogManagement.Shared.Models;

namespace WorklogManagement.UI.Components.Shared;

public partial class AttachmentPanel
{
    [Parameter]
    public IEnumerable<Attachment> Value { get; set; } = [];

    [Parameter]
    public EventCallback<IEnumerable<Attachment>> ValueChanged { get; set; }

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

        Value = attachments;
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task Edit(Attachment attachment)
    {
        Value = Value.Select(old => old.Name == attachment.Name ? attachment : old).ToArray();
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task Remove(Attachment attachment)
    {
        Value = Value.Where(a => a.Name != attachment.Name).ToArray();
        await ValueChanged.InvokeAsync(Value);
    }

    private async Task Download(Attachment attachment)
    {
        await JSRuntime.InvokeVoidAsync("downloadFile", attachment.Name, attachment.Data);
    }
}
