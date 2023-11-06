using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class TicketCommentAttachment : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri ticket comment

        [JsonPropertyName("ticketCommentId")]
        public int TicketCommentId { get; set; }

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
        private static readonly string _basedir = Path.Combine(".", "Attachments", "Tickets");
#else
        private static readonly string _basedir = Path.Combine("var", "www", "html", "_res", "WorklogManagement", "Attachments", "Tickets");
#endif

        [JsonConstructor]
        public TicketCommentAttachment(int? id, int ticketCommentId, string name, string comment, string directory, byte[] data)
        {
            Id = id;
            TicketCommentId = ticketCommentId;
            Name = name;
            Comment = comment;
            Directory = directory;
            Data = data;
        }

        public TicketCommentAttachment(DB.TicketCommentAttachment attachment)
        {
            Id = attachment.Id;
            TicketCommentId = attachment.TicketCommentId;
            Name = attachment.Name;
            Comment = attachment.Comment;

            Directory = Path.Combine(_basedir, attachment.TicketComment.TicketId.ToString(), TicketCommentId.ToString());
            Data = File.ReadAllBytes(Path.Combine(Directory!, Name));
        }

        public static async Task<TicketAttachment> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.TicketAttachments.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.TicketCommentAttachment attachment;

            if (Id == default)
            {
                await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Data);

                attachment = new()
                {
                    TicketCommentId = TicketCommentId,
                    Name = Name,
                    Comment = Comment,
                };

                await context.TicketCommentAttachments.AddAsync(attachment);

                await context.SaveChangesAsync();

                Id = attachment.Id;
            }
            else
            {
                attachment = await context.TicketCommentAttachments.SingleAsync(x => x.Id == Id);

                // alte Datei löschen
                File.Delete(Path.Combine(Directory, attachment.Name));

                // neue Datei speichern
                await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Data);

                attachment.TicketCommentId = TicketCommentId;
                attachment.Name = Name;
                attachment.Comment = Comment;

                await context.SaveChangesAsync();
            }
        }

        public static async Task DeleteAsync(WorklogManagementContext context, int id)
        {
            var attachment = await context.TicketCommentAttachments
                .Include(x => x.TicketComment)
                .SingleAsync(x => x.Id == id);

            context.TicketCommentAttachments.Remove(attachment);

            await context.SaveChangesAsync();

            TicketCommentAttachment ticketAttachment = new(attachment);

            File.Delete(Path.Combine(ticketAttachment.Directory, ticketAttachment.Name));
        }
    }
}
