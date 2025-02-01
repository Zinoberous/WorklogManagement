using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;
using DTO = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Worklogs;

public record Worklog : DTO.Worklog
{
    private int _id;
    public new int Id { get => _id; init => _id = value; }

    public new IEnumerable<WorklogAttachment> Attachments { get; init; } = [];

    // DTO > DB
    internal static Dictionary<string, string> PropertyMappings { get; } = new()
    {
        { "Id", "Id" },
        { "Date", "Date" },
        { "TicketId", "TicketId" },
        { "TicketTitle", "TicketTitle" },
        { "Description", "Description" },
        { "TimeSpent", "TimeSpentSeconds" },
    };

    internal static Worklog Map(DB.Worklog worklog)
    {
        return new()
        {
            Id = worklog.Id,
            Date = worklog.Date,
            TicketId = worklog.TicketId,
            TicketTitle = worklog.Ticket.Title,
            Description = worklog.Description,
            TimeSpent = worklog.TimeSpent,
            Attachments = worklog.WorklogAttachments
                .Select(WorklogAttachment.Map)
                .ToArray()
        };
    }

    internal async Task SaveAsync(WorklogManagementContext context)
    {
        var worklog = await context.Worklogs.SingleOrDefaultAsync(x => x.Id == _id);

        if (worklog is null)
        {
            worklog = new()
            {
                Date = Date,
                TicketId = TicketId,
                Description = Description,
                TimeSpent = TimeSpent
            };

            await context.Worklogs.AddAsync(worklog);

            await context.SaveChangesAsync();

            _id = worklog.Id;
        }
        else
        {
            worklog.Date = Date;
            worklog.TicketId = TicketId;
            worklog.Description = Description;
            worklog.TimeSpent = TimeSpent;

            await context.SaveChangesAsync();
        }

        foreach (var attachment in Attachments)
        {
            attachment.WorklogId = _id;
            await attachment.SaveAsync(context);
        }

        var deletedAttachments = await context.WorklogAttachments
            .Where(x => x.WorklogId == _id && !Attachments.Select(x => x.Id).Contains(x.Id))
            .ToListAsync();

        foreach (var attachment in deletedAttachments)
        {
            await WorklogAttachment.DeleteAsync(context, attachment.Id);
        }
    }

    internal static async Task DeleteAsync(WorklogManagementContext context, int id)
    {
        var worklog = await context.Worklogs
            .Include(x => x.WorklogAttachments)
            .SingleAsync(x => x.Id == id);

        foreach (var attachment in worklog.WorklogAttachments)
        {
            await WorklogAttachment.DeleteAsync(context, attachment.Id);
        }

        context.Worklogs.Remove(worklog);

        await context.SaveChangesAsync();
    }
}
