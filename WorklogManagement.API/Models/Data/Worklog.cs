using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models.Data
{
    public partial class Worklog : IData
    {
        public int? Id { get; private set; }

        public int DayId { get; set; }

        public int TicketId { get; set; }

        public string? Description { get; set; }

        public TimeSpan TimeSpent { get; set; }

        public string? TimeSpentComment { get; set; }

        [JsonConstructor]
        public Worklog(int? id, int dayId, int ticketId, string? description, TimeSpan timeSpent, string? timeSpentComment)
        {
            Id = id;
            DayId = dayId;
            TicketId = ticketId;
            Description = description;
            TimeSpent = timeSpent;
            TimeSpentComment = timeSpentComment;
        }

        public Worklog(DB.Worklog worklog)
        {
            Id = worklog.Id;
            DayId = worklog.DayId;
            TicketId = worklog.TicketId;
            Description = worklog.Description;
            TimeSpent = worklog.TimeSpent;
            TimeSpentComment = worklog.TimeSpentComment;
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
                    DayId = DayId,
                    TicketId = TicketId,
                    Description = Description,
                    TimeSpent = TimeSpent,
                    TimeSpentComment = TimeSpentComment,
                };

                await context.Worklogs.AddAsync(worklog);

                await context.SaveChangesAsync();

                Id = worklog.Id;
            }
            else
            {
                worklog = await context.Worklogs.SingleAsync(x => x.Id == Id);

                worklog.DayId = DayId;
                worklog.TicketId = TicketId;
                worklog.Description = Description;
                worklog.TimeSpent = TimeSpent;
                worklog.TimeSpentComment = TimeSpentComment;

                await context.SaveChangesAsync();
            }
        }
    }
}
