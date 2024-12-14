using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;
using WorklogManagement.Shared.Enums;
using WorklogManagement.Shared.Models;
using DB = WorklogManagement.Data.Models;

namespace WorklogManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class StatisticsController(WorklogManagementContext context) : ControllerBase
{
    private readonly WorklogManagementContext _context = context;

    [HttpGet("overtime")]
    public async Task<OvertimeInfo> GetOvertime()
    {
        var totalOvertime = TimeSpan.Zero;
        var officeOvertime = TimeSpan.Zero;
        var mobileOvertime = TimeSpan.Zero;

        var workTimes = await _context.WorkTimes
            .Where(x => x.ActualMinutes != x.ExpectedMinutes)
            .Select(x => new { Expected = x.ExpectedSpan, Actual = x.ActualSpan, x.WorkTimeTypeId })
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

    [HttpGet("calendar")]
    public async Task<Dictionary<CalendarEntryType, int>> GetCalendarStatics(int? year = null)
    {
        static Task<int> CountDistinctDatesAsync(IQueryable<CalendarEntry> entries)
        {
            return entries
                .Select(x => x.Date)
                .Distinct()
                .CountAsync();
        }

        var rawEntries = _context
            .WorkTimes.Select(x => new
            {
                Type = nameof(DB.WorkTime),
                TypeId = x.WorkTimeTypeId,
                x.Date,
                DurationMinutes = x.ActualMinutes,
            })
            .Union(_context.Absences.Select(x => new
            {
                Type = nameof(DB.Absence),
                TypeId = x.AbsenceTypeId,
                x.Date,
                x.DurationMinutes,
            }));

        var calendarEntries = rawEntries
            .Where(x => year == null || x.Date.Year == year)
            .Select(x => new CalendarEntry
            {
                Type = x.Type,
                TypeId = x.TypeId,
                Date = x.Date,
                DurationMinutes = x.DurationMinutes,
            });

        Dictionary<CalendarEntryType, IQueryable<CalendarEntry>> entries = new()
        {
            [CalendarEntryType.Workday] = calendarEntries,
            [CalendarEntryType.Office] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.WorkTime)
                    && x.TypeId == (int)WorkTimeType.Office
                    && x.DurationMinutes > 0),
            [CalendarEntryType.Mobile] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.WorkTime)
                    && x.TypeId == (int)WorkTimeType.Mobile
                    && x.DurationMinutes > 0),
            [CalendarEntryType.TimeCompensation] = calendarEntries
                .Where(x =>
                    x.Type == nameof(DB.WorkTime)
                    && x.DurationMinutes == 0),
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

    [HttpGet("tickets")]
    public async Task<Dictionary<TicketStatus, int>> GetTicketStatistics()
    {
        var statistics = await _context.Tickets
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
