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
            new Dictionary<CalendarEntryType, int>
            {
                [CalendarEntryType.Workday] =
                    await context.WorkTimes
                        .Where(x => year == null || x.Date.Year == year)
                        .Select(x => x.Date)
                        .Union(context.Absences
                            .Where(x => year == null || x.Date.Year == year)
                            .Select(x => x.Date))
                        .Distinct()
                        .CountAsync(),
                [CalendarEntryType.Office] =
                    await context.WorkTimes
                        .Where(x =>
                            (year == null || x.Date.Year == year)
                            && x.ActualMinutes > 0
                            && x.WorkTimeTypeId == (int)WorkTimeType.Office)
                        .Select(x => x.Date)
                        .Distinct()
                        .CountAsync(),
                [CalendarEntryType.Mobile] =
                    await context.WorkTimes
                        .Where(x =>
                            (year == null || x.Date.Year == year)
                            && x.ActualMinutes > 0
                            && x.WorkTimeTypeId == (int)WorkTimeType.Mobile)
                        .Select(x => x.Date)
                        .Distinct()
                        .CountAsync(),
                [CalendarEntryType.TimeCompensation] =
                    await context.WorkTimes
                        .Where(x =>
                            (year == null || x.Date.Year == year)
                            && x.ActualMinutes == 0)
                        .Select(x => x.Date)
                        .Distinct()
                        .CountAsync(),
                [CalendarEntryType.Holiday] =
                    await context.Absences
                        .Where(x =>
                            (year == null || x.Date.Year == year)
                            && x.AbsenceTypeId == (int)AbsenceType.Holiday)
                        .Select(x => x.Date)
                        .Distinct()
                        .CountAsync(),
                [CalendarEntryType.Vacation] =
                    await context.Absences
                        .Where(x =>
                            (year == null || x.Date.Year == year)
                            && x.AbsenceTypeId == (int)AbsenceType.Vacation)
                        .Select(x => x.Date)
                        .Distinct()
                        .CountAsync(),
                [CalendarEntryType.Ill] =
                    await context.Absences
                        .Where(x =>
                            (year == null || x.Date.Year == year)
                            && x.AbsenceTypeId == (int)AbsenceType.Ill)
                        .Select(x => x.Date)
                        .Distinct()
                        .CountAsync()
            }
        );
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
