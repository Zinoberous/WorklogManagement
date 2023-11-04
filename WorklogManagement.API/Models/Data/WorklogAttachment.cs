using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class WorklogAttachment : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri worklog

        [JsonPropertyName("worklogId")]
        public int WorklogId { get; set; }

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [JsonPropertyName("comment")]
        public string Comment { get; set; } = null!;

        [JsonPropertyName("directory")]
        public string Directory { get; set; }

        [JsonPropertyName("data")]
        public byte[] Data { get; set; }

#if DEBUG
        private static readonly string _basedir = Path.Combine(".", "Attachments", "Worklogs");
#else
        private static readonly string _basedir = Path.Combine("var", "www", "html", "_res", "WorklogManagement", "Attachments", "Worklogs");
#endif

        [JsonConstructor]
        public WorklogAttachment(int? id, int worklogId, string name, string comment, string directory, byte[] data)
        {
            Id = id;
            WorklogId = worklogId;
            Name = name;
            Comment = comment;
            Directory = directory;
            Data = data;
        }

        public WorklogAttachment(DB.WorklogAttachment attachment)
        {
            Id = attachment.Id;
            WorklogId = attachment.WorklogId;
            Name = attachment.Name;
            Comment = attachment.Comment;

            Directory = Path.Combine(_basedir, $"{attachment.Worklog.Day.Date:yyyy-MM-dd}-{(attachment.Worklog.Day.IsMobile ? "mobile" : "office")}", WorklogId.ToString()!);
            Data = File.ReadAllBytes(Path.Combine(Directory, Name));
        }

        public static async Task<WorklogAttachment> GetAsync(int id, WorklogManagementContext context)
        {
            var attachment = await context.WorklogAttachments
                .Include(x => x.Worklog)
                .ThenInclude(x => x.Day)
                .SingleAsync(x => x.Id == id);

            return new(attachment);
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.WorklogAttachment attachment;

            if (Id == default)
            {
                await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Data);

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

                // alte Datei löschen
                File.Delete(Path.Combine(Directory, attachment.Name));

                // neue Datei speichern
                await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Data);

                attachment.WorklogId = WorklogId;
                attachment.Name = Name;
                attachment.Comment = Comment;

                await context.SaveChangesAsync();
            }
        }
    }
}
