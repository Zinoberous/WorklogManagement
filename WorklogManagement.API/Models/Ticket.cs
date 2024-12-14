using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record Ticket : Shd.Ticket
{
    private int? _id;
    public new int? Id { get => _id; init => _id = value; }

    private DateTime? _createdAt;
    public new DateTime? CreatedAt { get => _createdAt; init => _createdAt = value; }

    internal static Ticket Map(DB.Ticket ticket)
    {
        return new()
        {
            Id = ticket.Id,
            RefId = ticket.RefId,
            Title = ticket.Title,
            Description = ticket.Description,
            Status = (TicketStatus)ticket.TicketStatusId,
            StatusNote = ticket.TicketStatusLogs.Last().Note,
            CreatedAt = ticket.CreatedAt,
            TimeSpent = TimeSpan.FromTicks(ticket.Worklogs.Sum(x => x.TimeSpent.Ticks)),
            AttachmentsCount = ticket.TicketAttachments.Count,
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        DB.Ticket ticket;

        if (_id is null)
        {
            ticket = new()
            {
                RefId = RefId,
                Title = Title,
                Description = Description,
                TicketStatusId = (int)Status,
                TicketStatusLogs =
                [
                    new()
                    {
                        TicketStatusId = (int)Status,
                        StartedAt = DateTime.UtcNow,
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
            ticket = await context.Tickets
                .Include(x => x.TicketStatusLogs)
                .SingleAsync(x => x.Id == _id);

            ticket.RefId = RefId;
            ticket.Title = Title;
            ticket.Description = Description;

            if (ticket.TicketStatusId != (int)Status)
            {
                ticket.TicketStatusId = (int)Status;

                DB.TicketStatusLog statusLog = new()
                {
                    TicketId = ticket.Id,
                    TicketStatusId = (int)Status,
                    StartedAt = DateTime.UtcNow
                };

                await context.TicketStatusLogs.AddAsync(statusLog);
            }

            if (ticket.TicketStatusLogs.Last().Note != StatusNote)
            {
                ticket.TicketStatusLogs.Last().Note = StatusNote;
            }

            await context.SaveChangesAsync();
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var ticket = await context.Tickets
            .Include(x => x.TicketAttachments)
            .SingleAsync(x => x.Id == id);

        await Parallel.ForEachAsync(ticket.TicketAttachments, async (attachment, _) =>
        {
            await TicketAttachment.DeleteAsync(context, attachment.Id);
        });

        context.Tickets.Remove(ticket);

        await context.SaveChangesAsync();
    }
}
