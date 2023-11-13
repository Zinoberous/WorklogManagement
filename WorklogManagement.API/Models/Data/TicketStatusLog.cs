using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class TicketStatusLog : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri ticket

        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("status")]
        public Enums.TicketStatus Status { get; set; }

        [JsonPropertyName("startedAt")]
        public DateTime? StartedAt { get; private set; }

        [JsonPropertyName("note")]
        public string? Note { get; set; }

        [JsonConstructor]
        public TicketStatusLog(int? id, int ticketId, Enums.TicketStatus status, DateTime? startedAt, string? note)
        {
            Id = id;
            TicketId = ticketId;
            Status = status;
            StartedAt = startedAt;
            Note = note;
        }

        public TicketStatusLog(DB.TicketStatusLog ticketStatusLog)
        {
            Id = ticketStatusLog.Id;
            TicketId = ticketStatusLog.TicketId;
            Status = (Enums.TicketStatus)ticketStatusLog.TicketStatusId;
            StartedAt = ticketStatusLog.StartedAt;
        }

        public static async Task<Ticket> GetAsync(int id, WorklogManagementContext context)
        {
            var ticket = await context.Tickets
                .SingleAsync(x => x.Id == id);

            return new(ticket);
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            var ticketStatusLog = await context.TicketStatusLogs
                .SingleAsync(x => x.Id == Id);

            ticketStatusLog.Note = Note;

            await context.SaveChangesAsync();
        }
    }
}
