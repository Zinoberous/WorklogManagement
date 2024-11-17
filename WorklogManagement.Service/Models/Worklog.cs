using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.Service.Models;

public class Worklog : IDataModel
{
    private int? _id;
    public int? Id { get => _id; init => _id = value; }

    public required DateOnly Date { get; init; }

    public required int TicketId { get; init; }

    public required string TicketTitle { get; init; }

    public string? Description { get; init; }

    public required int TimeSpentMinutes { get; init; }

    public int AttachmentsCount { get; init; }

    internal static Worklog Map(DB.Worklog worklog)
    {
        return new()
        {
            Id = worklog.Id,
            Date = worklog.Date,
            TicketId = worklog.TicketId,
            TicketTitle = worklog.Ticket.Title,
            Description = worklog.Description,
            TimeSpentMinutes = worklog.TimeSpentMinutes,
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
                TimeSpentMinutes = TimeSpentMinutes
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
            worklog.TimeSpentSeconds = TimeSpentMinutes;

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
