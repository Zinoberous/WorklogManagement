using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Models;

public partial class TicketStatusLog : Shd.TicketStatusLog
{
    internal static TicketStatusLog Map(DB.TicketStatusLog log)
    {
        return new()
        {
            Id = log.Id,
            TicketId = log.TicketId,
            TicketTitle = log.Ticket.Title,
            Status = (TicketStatus)log.TicketStatusId,
            StartedAt = log.StartedAt,
            Note = log.Note
        };
    }

    public async Task SaveAsync(WorklogManagementContext context)
    {
        // neue TicketStatusLog-Entries werden via Ticket.SaveAsync erstellt

        var log = await context.TicketStatusLogs.SingleAsync(x => x.Id == Id);

        log.Note = Note;

        await context.SaveChangesAsync();
    }
}
