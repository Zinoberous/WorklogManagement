using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Common;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record WorklogAttachment : Shd.WorklogAttachment
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    private string Directory => GetDirectory(WorklogId);

    private static string GetDirectory(int worklogId)
    {
        return Path.Combine
        (
            Configuration.AttachmentsBaseDir,
            "Worklogs",
            worklogId.ToString()
        );
    }

    internal static WorklogAttachment Map(DB.WorklogAttachment attachment)
    {
        var worklogId = attachment.WorklogId;
        var name = attachment.Name;

        return new()
        {
            Id = attachment.Id,
            WorklogId = worklogId,
            Name = name,
            Comment = attachment.Comment,
            Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(GetDirectory(worklogId), name)))
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        if (!System.IO.Directory.Exists(Directory))
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        var attachment = await context.WorklogAttachments.SingleOrDefaultAsync(x => x.Id == Id);

        if (attachment is null)
        {
            await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Convert.FromBase64String(Data));

            attachment = new()
            {
                WorklogId = WorklogId,
                Name = Name,
                Comment = Comment,
            };

            await context.WorklogAttachments.AddAsync(attachment);

            await context.SaveChangesAsync();

            _id = attachment.Id;
        }
        else
        {
            // alte Datei löschen
            File.Delete(Path.Combine(Directory, attachment.Name));

            // neue Datei speichern
            await File.WriteAllBytesAsync(Path.Combine(Directory, attachment.Name), Convert.FromBase64String(Data));

            attachment.WorklogId = WorklogId;
            attachment.Name = Name;
            attachment.Comment = Comment;

            await context.SaveChangesAsync();
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var attachment = await context.WorklogAttachments
            .SingleAsync(x => x.Id == id);

        context.WorklogAttachments.Remove(attachment);

        await context.SaveChangesAsync();

        File.Delete(Path.Combine(GetDirectory(attachment.WorklogId), attachment.Name));
    }
}
