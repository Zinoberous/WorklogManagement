using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public class TicketAttachment : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri ticket

        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("name")]
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("data")]
        public string Data { get; set; }

        private string Directory => Path.Combine(_basedir, TicketId.ToString());

#if DEBUG
        private static readonly string _basedir = Path.Combine(".", "Attachments", "Tickets");
#else
        private static readonly string _basedir = Path.Combine("/", "var", "www", "html", "_res", "WorklogManagement", "Attachments", "Tickets");
        //private static readonly string _basedir = Path.Combine("/", "var", "www", "html", "_res", "DevWorklogManagement", "Attachments", "Tickets");
#endif

        [JsonConstructor]
        public TicketAttachment(int? id, int ticketId, string name, string comment, string data)
        {
            Id = id;
            TicketId = ticketId;
            Name = name;
            Comment = comment;
            Data = data;
        }

        public TicketAttachment(DB.TicketAttachment attachment)
        {
            Id = attachment.Id;
            TicketId = attachment.TicketId;
            Name = attachment.Name;
            Comment = attachment.Comment;
            Data = Convert.ToBase64String(File.ReadAllBytes(Path.Combine(Directory, Name)));
        }

        public static async Task<TicketAttachment> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.TicketAttachments.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.TicketAttachment attachment;

            if (!System.IO.Directory.Exists(Directory))
            {
                System.IO.Directory.CreateDirectory(Directory);
            }

            if (Id == default)
            {
                await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Convert.FromBase64String(Data));

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

                // alte Datei löschen
                File.Delete(Path.Combine(Directory, attachment.Name));

                // neue Datei speichern
                await File.WriteAllBytesAsync(Path.Combine(Directory, Name), Convert.FromBase64String(Data));

                attachment.TicketId = TicketId;
                attachment.Name = Name;
                attachment.Comment = Comment;

                await context.SaveChangesAsync();
            }
        }

        public static async Task DeleteAsync(WorklogManagementContext context, int id)
        {
            var attachment = await context.TicketAttachments.SingleAsync(x => x.Id == id);

            context.TicketAttachments.Remove(attachment);

            await context.SaveChangesAsync();

            TicketAttachment ticketAttachment = new(attachment);

            File.Delete(Path.Combine(ticketAttachment.Directory, ticketAttachment.Name));
        }
    }
}
