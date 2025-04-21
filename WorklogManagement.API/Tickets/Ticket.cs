using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using DTO = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Tickets;

public record Ticket : DTO.Ticket
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    private DateTime? _createdAt;
    public new DateTime? CreatedAt { get => _createdAt; init => _createdAt = value; }

    public new IEnumerable<TicketAttachment> Attachments { get; init; } = [];

    // DTO > DB
    internal static Dictionary<string, string> PropertyMappings { get; } = new()
    {
        { "Id", "Id" },
        { "RefId", "RefId" },
        { "Title", "Title" },
        { "Description", "Description" },
        { "Status", "TicketStatusId" },
        { "CreatedAt", "CreatedAt" },
    };

    internal static Ticket Map(DB.Ticket ticket)
    {
        return new()
        {
            Id = ticket.Id,
            Ref = ticket.Ref is null ? null : new() { Id = ticket.Ref.Id, Title = ticket.Ref.Title },
            Title = ticket.Title,
            Description = ticket.Description,
            Status = (TicketStatus)ticket.TicketStatusId,
            StatusStartedAt = ticket.TicketStatusLogs.Last().StartedAt,
            StatusNote = ticket.TicketStatusLogs.Last().Note,
            CreatedAt = ticket.CreatedAt,
            TimeSpent = TimeSpan.FromTicks(ticket.Worklogs.Sum(x => x.TimeSpent.Ticks)),
            Attachments = [.. ticket.TicketAttachments.Select(TicketAttachment.Map)],
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        var ticket = await context.Tickets
            .Include(x => x.TicketStatusLogs)
            .SingleOrDefaultAsync(x => x.Id == _id);

        if (ticket is null)
        {
            ticket = new()
            {
                RefId = Ref?.Id,
                Title = Title,
                Description = Description,
                TicketStatusId = (int)Status,
                TicketStatusLogs = [
                    new()
                    {
                        TicketStatusId = (int)Status,
                        StartedAt = DateTime.UtcNow,
                        Note = StatusNote,
                    }
                ],
                CreatedAt = DateTime.UtcNow,
            };

            await context.Tickets.AddAsync(ticket);

            await context.SaveChangesAsync();

            _id = ticket.Id;
            _createdAt = ticket.CreatedAt;
        }
        else
        {
            ticket.RefId = Ref?.Id;
            ticket.Title = Title;
            ticket.Description = Description;

            if (ticket.TicketStatusId != (int)Status)
            {
                ticket.TicketStatusId = (int)Status;

                DB.TicketStatusLog statusLog = new()
                {
                    TicketId = ticket.Id,
                    TicketStatusId = (int)Status,
                    StartedAt = DateTime.UtcNow,
                    Note = StatusNote,
                };

                await context.TicketStatusLogs.AddAsync(statusLog);
            }

            if (ticket.TicketStatusLogs.Last().Note != StatusNote)
            {
                ticket.TicketStatusLogs.Last().Note = StatusNote;
            }

            await context.SaveChangesAsync();
        }

        foreach (var attachment in Attachments)
        {
            attachment.TicketId = _id;
            await attachment.SaveAsync(context);
        }

        var deletedAttachments = await context.TicketAttachments
            .Where(x => x.TicketId == _id && !Attachments.Select(x => x.Id).Contains(x.Id))
            .ToListAsync();

        foreach (var attachment in deletedAttachments)
        {
            await TicketAttachment.DeleteAsync(context, attachment.Id);
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var ticket = await context.Tickets
            .Include(x => x.TicketAttachments)
            .SingleAsync(x => x.Id == id);

        foreach (var attachment in ticket.TicketAttachments)
        {
            await TicketAttachment.DeleteAsync(context, attachment.Id);
        }

        context.Tickets.Remove(ticket);

        await context.SaveChangesAsync();
    }
}
