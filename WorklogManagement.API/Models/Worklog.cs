using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public record Worklog : Shd.Worklog
{
    private int? _id;
    public new int? Id { get => _id; init => _id = value; }

    internal static Worklog Map(DB.Worklog worklog)
    {
        return new()
        {
            Id = worklog.Id,
            Date = worklog.Date,
            TicketId = worklog.TicketId,
            TicketTitle = worklog.Ticket.Title,
            Description = worklog.Description,
            TimeSpent = worklog.TimeSpentSpan,
            AttachmentsCount = worklog.WorklogAttachments.Count
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        DB.Worklog worklog;

        if (_id is null)
        {
            worklog = new()
            {
                Date = Date,
                TicketId = TicketId,
                Description = Description,
                TimeSpentSpan = TimeSpent
            };

            await context.Worklogs.AddAsync(worklog);

            await context.SaveChangesAsync();

            _id = worklog.Id;
        }
        else
        {
            worklog = await context.Worklogs.SingleAsync(x => x.Id == _id);

            worklog.Date = Date;
            worklog.TicketId = TicketId;
            worklog.Description = Description;
            worklog.TimeSpentSpan = TimeSpent;

            await context.SaveChangesAsync();
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var worklog = await context.Worklogs
            .Include(x => x.WorklogAttachments)
            .SingleAsync(x => x.Id == id);

        await Parallel.ForEachAsync(worklog.WorklogAttachments, async (attachment, _) =>
        {
            await WorklogAttachment.DeleteAsync(context, attachment.Id);
        });

        context.Worklogs.Remove(worklog);

        await context.SaveChangesAsync();
    }
}
