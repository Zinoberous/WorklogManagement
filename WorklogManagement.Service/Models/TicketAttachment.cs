using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using WorklogManagement.Data.Context;
using WorklogManagement.Service.Common;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.Service.Models;

public class TicketAttachment
{
    private int? _Id;
    public int? Id { get => _Id; init => _Id = value; }

    public int TicketId { get; init; }

    [MaxLength(255)]
    public required string Name { get; init; }

    public string? Comment { get; init; }

    public required string Data { get; init; }

    private string Directory => GetDirectory(TicketId);

    private static string GetDirectory(int ticketId)
    {
        return Path.Combine
        (
            Configuration.AttachmentsBaseDir,
            "Tickets",
            ticketId.ToString()
        );
    }

    internal static TicketAttachment Map(DB.TicketAttachment attachment)
    {
        var ticketId = attachment.TicketId;
        var name = attachment.Name;

        return new()
        {
            Id = attachment.Id,
            TicketId = ticketId,
            Name = name,
            Comment = attachment.Comment,
            Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(GetDirectory(ticketId), name)))
        };
    }

    public async Task SaveAsync(WorklogManagementContext context)
    {
        DB.TicketAttachment attachment;

        if (!System.IO.Directory.Exists(Directory))
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        if (_Id is null)
        {
            await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Convert.FromBase64String(Data));

            attachment = new()
            {
                TicketId = TicketId,
                Name = Name,
                Comment = Comment,
            };

            await context.TicketAttachments.AddAsync(attachment);

            await context.SaveChangesAsync();

            _Id = attachment.Id;
        }
        else
        {
            attachment = await context.TicketAttachments.SingleAsync(x => x.Id == _Id);

            // alte Datei löschen
            File.Delete(Path.Combine(Directory, attachment.Name));

            // neue Datei speichern
            await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Convert.FromBase64String(Data));

            attachment.TicketId = TicketId;
            attachment.Name = Name;
            attachment.Comment = Comment;

            await context.SaveChangesAsync();
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var attachment = await context.TicketAttachments.SingleAsync(x => x.Id == id);

        context.TicketAttachments.Remove(attachment);

        await context.SaveChangesAsync();

        File.Delete(Path.Combine(GetDirectory(attachment.TicketId), attachment.Name));
    }
}
