using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class TicketComment
    {
        public int? Id { get; private set; }

        public int TicketId { get; set; }

        public string Comment { get; set; } = null!;

        public DateTime? CreatedAt { get; private set; }

        [JsonConstructor]
        public TicketComment(int? id, int ticketId, string comment, DateTime? createdAt)
        {
            Id = id;
            TicketId = ticketId;
            Comment = comment;
            CreatedAt = createdAt;
        }

        public TicketComment(DB.TicketComment comment)
        {
            Id = comment.Id;
            TicketId = comment.TicketId;
            Comment = comment.Comment;
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
                    Comment = Comment,
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
                comment.Comment = Comment;

                await context.SaveChangesAsync();
            }
        }
    }
}
