using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.API.Statistics;

internal static class StatisticEndpoints
{
    internal static IEndpointRouteBuilder RegisterStatisticEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/statistics").WithTags("Statistics");

        group.MapGet("/overtime", GetOvertime);
        group.MapGet("/calendar", GetCalendarStatics);
        group.MapGet("/tickets", GetTicketStatistics);

        return app;
    }

    private static async Task<OvertimeInfo> GetOvertime(WorklogManagementContext context)
    {
        var totalOvertime = TimeSpan.Zero;
        var officeOvertime = TimeSpan.Zero;
        var mobileOvertime = TimeSpan.Zero;

        var workTimes = await context.WorkTimes
            .Where(x => x.ActualSeconds != x.ExpectedSeconds)
            .Select(x => new { x.Expected, x.Actual, x.WorkTimeTypeId })
            .ToListAsync();

        object lockObj = new();

        workTimes.AsParallel().ForAll(entry =>
        {
            var expected = entry.Expected;
            var actual = entry.Actual;

            var overtime = actual - expected;

            lock (lockObj)
            {
                totalOvertime += overtime;

                if (entry.WorkTimeTypeId == (int)WorkTimeType.Office)
                {
                    officeOvertime += overtime;
                }
                else if (entry.WorkTimeTypeId == (int)WorkTimeType.Mobile)
                {
                    mobileOvertime += overtime;
                }
            }
        });

        return new()
        {
            Total = totalOvertime,
            Office = officeOvertime,
            Mobile = mobileOvertime,
        };
    }

    private static async Task<IDictionary<CalendarEntryType, int>> GetCalendarStatics(WorklogManagementContext context, int? year = null)
    {
        static Task<int> CountDistinctDatesAsync(IQueryable<CalendarEntry> entries)
        {
            return entries
                .Select(x => x.Date)
                .Distinct()
                .CountAsync();
        }

        var rawEntries = context
            .WorkTimes.Select(x => new
            {
                Type = nameof(DB.WorkTime),
                TypeId = x.WorkTimeTypeId,
                x.Date,
                DurationSeconds = x.ActualSeconds,
            })
            .Union(context.Absences.Select(x => new
            {
                Type = nameof(DB.Absence),
                TypeId = x.AbsenceTypeId,
                x.Date,
                x.DurationSeconds,
            }));

        var calendarEntries = rawEntries
            .Where(x => year == null || x.Date.Year == year)
            .Select(x => new CalendarEntry
            {
                Type = x.Type,
                TypeId = x.TypeId,
                Date = x.Date,
                DurationSeconds = x.DurationSeconds,
            });

        Dictionary<CalendarEntryType, IQueryable<CalendarEntry>> entries = new()
        {
            [CalendarEntryType.Workday] = calendarEntries,
            [CalendarEntryType.Office] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.WorkTime)
                    && x.TypeId == (int)WorkTimeType.Office
                    && x.DurationSeconds > 0),
            [CalendarEntryType.Mobile] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.WorkTime)
                    && x.TypeId == (int)WorkTimeType.Mobile
                    && x.DurationSeconds > 0),
            [CalendarEntryType.TimeCompensation] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.WorkTime)
                    && x.DurationSeconds == 0),
            [CalendarEntryType.Holiday] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.Absence)
                    && x.TypeId == (int)AbsenceType.Holiday),
            [CalendarEntryType.Vacation] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.Absence)
                    && x.TypeId == (int)AbsenceType.Vacation),
            [CalendarEntryType.Ill] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.Absence)
                    && x.TypeId == (int)AbsenceType.Ill)
        };

        Dictionary<CalendarEntryType, int> result = [];

        foreach (var entry in entries)
        {
            result[entry.Key] = await CountDistinctDatesAsync(entry.Value);
        }

        return result;
    }

    private static async Task<IDictionary<TicketStatus, int>> GetTicketStatistics(WorklogManagementContext context)
    {
        var statistics = await context.Tickets
            .GroupBy(x => x.TicketStatusId)
            .ToDictionaryAsync(x => (TicketStatus)x.Key, x => x.Count());

        var ticketStatuses = Enum.GetValues<TicketStatus>();

        foreach (var status in ticketStatuses)
        {
            if (!statistics.ContainsKey(status))
            {
                statistics[status] = 0;
            }
        }

        return statistics;
    }
}
