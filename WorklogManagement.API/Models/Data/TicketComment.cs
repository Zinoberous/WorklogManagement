using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class TicketComment : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri ticket

        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; } = null!;

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; private set; }

        [JsonConstructor]
        public TicketComment(int? id, int ticketId, string description, DateTime? createdAt)
        {
            Id = id;
            TicketId = ticketId;
            Description = description;
            CreatedAt = createdAt;
        }

        public TicketComment(DB.TicketComment comment)
        {
            Id = comment.Id;
            TicketId = comment.TicketId;
            Description = comment.Description;
            CreatedAt = comment.CreatedAt;
        }

        public static async Task<TicketComment> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.TicketComments.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.TicketComment comment;

            if (Id == default)
            {
                comment = new()
                {
                    TicketId = TicketId,
                    Description = Description,
                };

                await context.TicketComments.AddAsync(comment);

                await context.SaveChangesAsync();

                Id = comment.Id;
                CreatedAt = comment.CreatedAt;
            }
            else
            {
                comment = await context.TicketComments.SingleAsync(x => x.Id == Id);

                comment.TicketId = TicketId;
                comment.Description = Description;

                await context.SaveChangesAsync();
            }
        }
    }
}
