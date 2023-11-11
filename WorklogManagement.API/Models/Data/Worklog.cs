using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
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
        public DateTime Date { get; set; }

        // TODO: uri ticket

        [JsonPropertyName("ticketId")]
        public int TicketId { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("timeSpent")]
        public int TimeSpentSeconds { get; set; }

        // TODO: uri attachments

        [JsonConstructor]
        public Worklog(int? id, DateTime date, int ticketId, string? description, int timeSpentSeconds)
        {
            Id = id;
            Date = date;
            TicketId = ticketId;
            Description = description;
            TimeSpentSeconds = timeSpentSeconds;
        }

        public Worklog(DB.Worklog worklog)
        {
            Id = worklog.Id;
            Date = worklog.Date;
            TicketId = worklog.TicketId;
            Description = worklog.Description;
            TimeSpentSeconds = (int)worklog.TimeSpent.TotalSeconds;
        }

        public static async Task<Worklog> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.Worklogs.SingleAsync(x => x.Id == id));
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
                    TimeSpent = TimeSpan.FromSeconds(TimeSpentSeconds),
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
                worklog.TimeSpent = TimeSpan.FromSeconds(TimeSpentSeconds);

                await context.SaveChangesAsync();
            }
        }
    }
}
