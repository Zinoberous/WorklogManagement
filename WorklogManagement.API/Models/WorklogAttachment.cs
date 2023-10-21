using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models
{
    public class WorklogAttachment
    {
        public int? Id { get; private set; }

        public int WorklogId { get; set; }

        [StringLength(255)]
        public string Name { get; set; } = null!;

        public string Comment { get; set; } = null!;

        [JsonConstructor]
        public WorklogAttachment(int? id, int worklogId, string name, string comment)
        {
            Id = id;
            WorklogId = worklogId;
            Name = name;
            Comment = comment;
        }

        public WorklogAttachment(DB.WorklogAttachment attachment)
        {
            Id = attachment.Id;
            WorklogId = attachment.WorklogId;
            Name = attachment.Name;
            Comment = attachment.Comment;
        }

        public static async Task<WorklogAttachment> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.WorklogAttachments.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.WorklogAttachment attachment;

            if (Id == default)
            {
                attachment = new()
                {
                    WorklogId = WorklogId,
                    Name = Name,
                    Comment = Comment,
                };

                await context.WorklogAttachments.AddAsync(attachment);

                await context.SaveChangesAsync();

                Id = attachment.Id;
            }
            else
            {
                attachment = await context.WorklogAttachments.SingleAsync(x => x.Id == Id);

                attachment.WorklogId = WorklogId;
                attachment.Name = Name;
                attachment.Comment = Comment;

                await context.SaveChangesAsync();
            }
        }
    }
}
