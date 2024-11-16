using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Service.Common;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.Service.Models;

public class WorklogAttachment
{
    private int? _Id;
    public int? Id { get => _Id; init => _Id = value; }

    public required int WorklogId { get; init; }

    public required string Name { get; init; }

    public string? Comment { get; init; }

    public required string Data { get; init; }

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

    public async Task SaveAsync(WorklogManagementContext context)
    {
        DB.WorklogAttachment attachment;

        if (!System.IO.Directory.Exists(Directory))
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        if (_Id is null)
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

            _Id = attachment.Id;
        }
        else
        {
            attachment = await context.WorklogAttachments.SingleAsync(x => x.Id == Id);

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

    public static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var attachment = await context.WorklogAttachments.SingleAsync(x => x.Id == id);

        context.WorklogAttachments.Remove(attachment);

        await context.SaveChangesAsync();

        File.Delete(Path.Combine(GetDirectory(attachment.WorklogId), attachment.Name));
    }
}
