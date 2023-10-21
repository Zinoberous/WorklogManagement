using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models
{
    public class TicketAttachment
    {
        public int? Id { get; private set; }

        public int TicketId { get; set; }

        [StringLength(255)]
        public string Name { get; set; } = null!;

        public string Comment { get; set; } = null!;

        [JsonConstructor]
        public TicketAttachment(int? id, int ticketId, string name, string comment)
        {
            Id = id;
            TicketId = ticketId;
            Name = name;
            Comment = comment;
        }

        public TicketAttachment(DB.TicketAttachment attachment)
        {
            Id = attachment.Id;
            TicketId = attachment.TicketId;
            Name = attachment.Name;
            Comment = attachment.Comment;
        }

        public static async Task<TicketAttachment> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.TicketAttachments.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.TicketAttachment attachment;

            if (Id == default)
            {
                attachment = new()
                {
                    TicketId = TicketId,
                    Name = Name,
                    Comment = Comment,
                };

                await context.TicketAttachments.AddAsync(attachment);

                await context.SaveChangesAsync();

                Id = attachment.Id;
            }
            else
            {
                attachment = await context.TicketAttachments.SingleAsync(x => x.Id == Id);

                attachment.TicketId = TicketId;
                attachment.Name = Name;
                attachment.Comment = Comment;

                await context.SaveChangesAsync();
            }
        }
    }
}
