using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Common;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Tickets;

public record TicketAttachment : Shd.TicketAttachment
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

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

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        if (!System.IO.Directory.Exists(Directory))
        {
            System.IO.Directory.CreateDirectory(Directory);
        }

        var attachment = await context.TicketAttachments.SingleOrDefaultAsync(x => x.Id == _id);

        if (attachment is null)
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

            _id = attachment.Id;
        }
        else
        {
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
        var attachment = await context.TicketAttachments
            .SingleAsync(x => x.Id == id);

        context.TicketAttachments.Remove(attachment);

        await context.SaveChangesAsync();

        File.Delete(Path.Combine(GetDirectory(attachment.TicketId), attachment.Name));
    }
}
