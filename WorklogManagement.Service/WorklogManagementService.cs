using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WorklogManagement.Data.Context;
using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;

namespace WorklogManagement.Service;

public class WorklogManagementService(IDbContextFactory<WorklogManagementContext> factory) : IWorklogManagementService
{
    private readonly IDbContextFactory<WorklogManagementContext> _factory = factory;

    public async Task<OvertimeInfo> GetOvertimeAsync()
    {
        var totalOvertimeMinutes = 0;
        var officeOvertimeMinutes = 0;
        var mobileOvertimeMinutes = 0;

        var workTimes = await ExecuteAsync(context =>
            context.WorkTimes
                .Where(x => x.ActualMinutes != x.ExpectedMinutes)
                .Select(x => new { x.ExpectedMinutes, x.ActualMinutes, x.WorkTimeTypeId })
                .ToListAsync()
        );

        workTimes.AsParallel().ForAll(entry =>
        {
            var expectedMinutes = entry.ExpectedMinutes;
            var actualMinutes = entry.ActualMinutes;

            var overtimeMinutes = (actualMinutes - expectedMinutes);

            Interlocked.Add(ref totalOvertimeMinutes, overtimeMinutes);

            if (entry.WorkTimeTypeId == (int)WorkTimeType.Office)
            {
                Interlocked.Add(ref officeOvertimeMinutes, overtimeMinutes);
            }
            else if (entry.WorkTimeTypeId == (int)WorkTimeType.Mobile)
            {
                Interlocked.Add(ref mobileOvertimeMinutes, overtimeMinutes);
            }
        });

        return new()
        {
            TotalMinutes = totalOvertimeMinutes,
            OfficeMinutes = officeOvertimeMinutes,
            MobileMinutes = mobileOvertimeMinutes,
        };
    }

    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStaticsAsync(int? year = null)
    {
        return await ExecuteAsync(async context =>
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
                    Type = nameof(WorkTime),
                    TypeId = x.WorkTimeTypeId,
                    DurationMinutes = x.ActualMinutes,
                    x.Date
                })
                .Union(context.Absences.Select(x => new
                {
                    Type = nameof(Absence),
                    TypeId = x.AbsenceTypeId,
                    x.DurationMinutes,
                    x.Date
                }));

            var calendarEntries = rawEntries
                .Where(x => year == null || x.Date.Year == year)
                .Select(x => new CalendarEntry
                {
                    Type = x.Type,
                    TypeId = x.TypeId,
                    DurationMinutes = x.DurationMinutes,
                    Date = x.Date
                });

            Dictionary<CalendarEntryType, IQueryable<CalendarEntry>> entries = new()
            {
                [CalendarEntryType.Workday] = calendarEntries,
                [CalendarEntryType.Office] = calendarEntries
                    .Where(x =>
                        x.Type == nameof(WorkTime)
                        && x.TypeId == (int)WorkTimeType.Office
                        && x.DurationMinutes > 0),
                [CalendarEntryType.Mobile] = calendarEntries
                    .Where(x =>
                        x.Type == nameof(WorkTime)
                        && x.TypeId == (int)WorkTimeType.Mobile
                        && x.DurationMinutes > 0),
                [CalendarEntryType.TimeCompensation] = calendarEntries
                    .Where(x =>
                        x.Type == nameof(WorkTime)
                        && x.DurationMinutes == 0),
                [CalendarEntryType.Holiday] = calendarEntries
                    .Where(x =>
                        x.Type == nameof(Absence)
                        && x.TypeId == (int)AbsenceType.Holiday),
                [CalendarEntryType.Vacation] = calendarEntries
                    .Where(x =>
                        x.Type == nameof(Absence)
                        && x.TypeId == (int)AbsenceType.Vacation),
                [CalendarEntryType.Ill] = calendarEntries
                    .Where(x =>
                        x.Type == nameof(Absence)
                        && x.TypeId == (int)AbsenceType.Ill)
            };

            Dictionary<CalendarEntryType, int> result = [];

            foreach (var entry in entries)
            {
                result[entry.Key] = await CountDistinctDatesAsync(entry.Value);
            }

            return result;
        });
    }

    public async Task<Dictionary<TicketStatus, int>> GetTicketStatisticsAsync()
    {
        var statistics = await ExecuteAsync(context =>
            context.Tickets
                .GroupBy(x => x.TicketStatusId)
                .ToDictionaryAsync(x => (TicketStatus)x.Key, x => x.Count())
        );

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

    public async Task<List<WorkTime>> GetWorkTimesAsync(DateOnly from, DateOnly to)
    {
        return await ExecuteAsync(context =>
            context.WorkTimes
                .Where(x => x.Date >= from && x.Date <= to)
                .Select(x => WorkTime.Map(x))
                .ToListAsync()
        );
    }

    public async Task<List<Absence>> GetAbsencesAsync(DateOnly from, DateOnly to)
    {
        return await ExecuteAsync(context =>
            context.Absences
                .Where(x => x.Date >= from && x.Date <= to)
                .Select(x => Absence.Map(x))
                .ToListAsync()
        );
    }

    public async Task<TDataModel> SaveAsync<TDataModel>(TDataModel item)
        where TDataModel : IDataModel
    {
        await ExecuteAsync(context =>
            (Task)typeof(TDataModel)
                .GetMethod("SaveAsync", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(item, [context])!
        );

        return item;
    }

    public async Task DeleteAsync<TDataModel>(int id)
        where TDataModel : IDataModel
    {
        await ExecuteAsync(context =>
            (Task)typeof(TDataModel)
                .GetMethod("DeleteAsync", BindingFlags.Static | BindingFlags.NonPublic)!
                .Invoke(null, [context, id])!
        );
    }

    private async Task ExecuteAsync(Func<WorklogManagementContext, Task> action)
    {
        using var context = _factory.CreateDbContext();
        await action(context);
    }

    private async Task<T> ExecuteAsync<T>(Func<WorklogManagementContext, Task<T>> action)
    {
        using var context = _factory.CreateDbContext();
        return await action(context);
    }
}
