using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class Ticket : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri ref

        [JsonPropertyName("refId")]
        public int? RefId { get; set; }

        [JsonPropertyName("title")]
        [MaxLength(255)]
        public string Title { get; set; } = null!;

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("status")]
        public Enums.TicketStatus Status { get; set; }

        [JsonPropertyName("statusNote")]
        public string? StatusNote { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; private set; }

        // TODO: uri attachments

        [JsonPropertyName("attachmentsCount")]
        public int AttachmentsCount { get; set; }

        // TODO: uri worklogs

        [JsonPropertyName("timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }

        [JsonConstructor]
        public Ticket(int? id, int? refId, string title, string? description, Enums.TicketStatus status, string? statusNote, DateTime? createdAt, int attachmentsCount, int timeSpentSeconds)
        {
            Id = id;
            RefId = refId;
            Title = title;
            Description = description;
            Status = status;
            StatusNote = statusNote;
            CreatedAt = createdAt;
            AttachmentsCount = attachmentsCount;
            TimeSpentSeconds = timeSpentSeconds;
        }

        public Ticket(DB.Ticket ticket)
        {
            Id = ticket.Id;
            RefId = ticket.RefId;
            Title = ticket.Title;
            Description = ticket.Description;
            Status = (Enums.TicketStatus)ticket.TicketStatusId;
            StatusNote = ticket.TicketStatusLogs.Last().Note;
            CreatedAt = ticket.CreatedAt;
            AttachmentsCount = ticket.TicketAttachments.Count;
            TimeSpentSeconds = ticket.Worklogs.Sum(x => x.TimeSpentSeconds);
        }

        public static async Task<Ticket> GetAsync(int id, WorklogManagementContext context)
        {
            var ticket = await context.Tickets
                .Include(x => x.TicketStatusLogs)
                .Include(x => x.TicketAttachments)
                .Include(x => x.Worklogs)
                .SingleAsync(x => x.Id == id);

            return new(ticket);
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.Ticket ticket;

            if (Id == default)
            {
                ticket = new()
                {
                    RefId = RefId,
                    Title = Title,
                    Description = Description,
                    TicketStatusId = (int)Status,
                    TicketStatusLogs = new List<DB.TicketStatusLog>
                    {
                        new()
                        {
                            TicketStatusId = (int)Status,
                            StartedAt = DateTime.UtcNow,
                        }
                    },
                    CreatedAt = DateTime.UtcNow,
                };

                await context.Tickets.AddAsync(ticket);

                await context.SaveChangesAsync();

                Id = ticket.Id;
                CreatedAt = ticket.CreatedAt;
            }
            else
            {
                ticket = await context.Tickets
                    .Include(x => x.TicketStatusLogs)
                    .SingleAsync(x => x.Id == Id);

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
    }
}
