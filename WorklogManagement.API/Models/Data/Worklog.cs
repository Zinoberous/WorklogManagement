using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.API.Helper;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public partial class Worklog : IData
    {
        // TODO: uri self

        [JsonPropertyName("id")]
        public int? Id { get; private set; }

        // TODO: uri day

        [JsonPropertyName("date")]
        public DateOnly Date { get; set; }

        // TODO: uri ticket

        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("ticket")]
        public string Ticket { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("timeSpentSeconds")]
        public int TimeSpentSeconds { get; set; }

        // TODO: uri attachments

        [JsonPropertyName("attachmentsCount")]
        public int AttachmentsCount { get; set; }

        [JsonConstructor]
        public Worklog(int? id, DateOnly date, int ticketId, string ticket, string? description, int timeSpentSeconds, int attachmentsCount)
        {
            Id = id;
            Date = date;
            TicketId = ticketId;
            Ticket = ticket;
            Description = description;
            TimeSpentSeconds = timeSpentSeconds;
            AttachmentsCount = attachmentsCount;
        }

        public Worklog(DB.Worklog worklog)
        {
            Id = worklog.Id;
            Date = worklog.Date;
            TicketId = worklog.TicketId;
            Ticket = worklog.Ticket.Title;
            Description = worklog.Description;
            TimeSpentSeconds = worklog.TimeSpentSeconds;
            AttachmentsCount = worklog.WorklogAttachments.Count;
        }

        public static async Task<Worklog> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.Worklogs.Include(x => x.Ticket).Include(x => x.WorklogAttachments).SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.Worklog worklog;

            if (Id == default)
            {
                worklog = new()
                {
                    Date = Date,
                    TicketId = TicketId,
                    Description = Description,
                    TimeSpent = TimeHelper.SecondsToTime(TimeSpentSeconds),
                    TimeSpentSeconds = TimeSpentSeconds
                };

                await context.Worklogs.AddAsync(worklog);

                await context.SaveChangesAsync();

                Id = worklog.Id;
            }
            else
            {
                worklog = await context.Worklogs.SingleAsync(x => x.Id == Id);

                worklog.Date = Date;
                worklog.TicketId = TicketId;
                worklog.Description = Description;
                worklog.TimeSpent = TimeHelper.SecondsToTime(TimeSpentSeconds);
                worklog.TimeSpentSeconds = TimeSpentSeconds;

                await context.SaveChangesAsync();
            }
        }
    }
}
