using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.Service.Models;

public class TicketStatusLog : IDataModel
{
    public int Id { get; init; }

    public required int TicketId { get; init; }

    public required string TicketTitle { get; init; }

    public required Enums.TicketStatus Status { get; init; }

    public required DateTime StartedAt { get; init; }

    public string? Note { get; init; }

    internal static TicketStatusLog Map(DB.TicketStatusLog log)
    {
        return new()
        {
            Id = log.Id,
            TicketId = log.TicketId,
            TicketTitle = log.Ticket.Title,
            Status = (Enums.TicketStatus)log.TicketStatusId,
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
