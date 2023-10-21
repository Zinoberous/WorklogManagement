using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WorklogManagement.DataAccess.Context;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Models
{
    public class Day
    {
        public int? Id { get; private set; }

        public DateTime Date { get; set; }

        public bool IsMobile { get; set; }

        public Enums.Workload Workload { get; set; }

        public string? WorkloadComment { get; set; }

        [JsonConstructor]
        public Day(int? id, DateTime date, bool isMobile, Enums.Workload workload, string? workloadComment)
        {
            Id = id;
            Date = date;
            IsMobile = isMobile;
            Workload = workload;
            WorkloadComment = workloadComment;
        }

        public Day(DB.Day day)
        {
            Id = day.Id;
            Date = day.Date;
            IsMobile = day.IsMobile;
            Workload = (Enums.Workload)(day.WorkloadId ?? 0);
            WorkloadComment = day.WorkloadComment;
        }

        public static async Task<Day> GetAsync(int id, WorklogManagementContext context)
        {
            return new(await context.Days.SingleAsync(x => x.Id == id));
        }

        public async Task SaveAsync(WorklogManagementContext context)
        {
            DB.Day day;

            if (Id == default)
            {
                day = new()
                {
                    Date = Date,
                    IsMobile = IsMobile,
                    WorkloadId = Workload == Enums.Workload.NoAnswer ? null : (int)Workload,
                    WorkloadComment = WorkloadComment,
                };

                await context.Days.AddAsync(day);

                await context.SaveChangesAsync();

                Id = day.Id;
            }
            else
            {
                day = await context.Days.SingleAsync(x => x.Id == Id);

                day.Date = Date;
                day.IsMobile = IsMobile;
                day.WorkloadId = Workload == Enums.Workload.NoAnswer ? null : (int)Workload;
                day.WorkloadComment = WorkloadComment;

                await context.SaveChangesAsync();
            }
        }
    }
}
