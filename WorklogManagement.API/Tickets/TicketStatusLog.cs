using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using DB = WorklogManagement.Data.Models;
using Shd = WorklogManagement.Shared.Models;

namespace WorklogManagement.API.Tickets;

public record TicketStatusLog : Shd.TicketStatusLog
{
    // Shd > DB
    internal static Dictionary<string, string> PropertyMappings { get; } = new()
    {
        { "Id", "Id" },
        { "TicketId", "TicketId" },
        { "Title", "TicketTitle" },
        { "Status", "TicketStatusId" },
        { "StartedAt", "StartedAt" },
        { "Note", "Note" },
    };

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
