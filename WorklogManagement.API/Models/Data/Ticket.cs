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

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; private set; }

        // TODO: uri attachments

        // TODO: uri worklogs

        [JsonConstructor]
        public Ticket(int? id, int? refId, string title, string? description, Enums.TicketStatus status, DateTime? createdAt)
        {
            Id = id;
            RefId = refId;
            Title = title;
            Description = description;
            Status = status;
            CreatedAt = createdAt;
        }

        public Ticket(DB.Ticket ticket)
        {
            Id = ticket.Id;
            RefId = ticket.RefId;
            Title = ticket.Title;
            Description = ticket.Description;
            Status = (Enums.TicketStatus)ticket.TicketStatusId;
            CreatedAt = ticket.CreatedAt;
        }

        public static async Task<Ticket> GetAsync(int id, WorklogManagementContext context)
        {
            var ticket = await context.Tickets
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
                };

                await context.Tickets.AddAsync(ticket);

                await context.SaveChangesAsync();

                Id = ticket.Id;
                CreatedAt = ticket.CreatedAt;
            }
            else
            {
                ticket = await context.Tickets.SingleAsync(x => x.Id == Id);

                ticket.RefId = RefId;
                ticket.Title = Title;
                ticket.Description = Description;
                ticket.TicketStatusId = (int)Status;

                await context.SaveChangesAsync();
            }
        }
    }
}
