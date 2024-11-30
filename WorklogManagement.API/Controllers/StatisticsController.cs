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
        var totalOvertimeMinutes = 0;
        var officeOvertimeMinutes = 0;
        var mobileOvertimeMinutes = 0;

        var workTimes = await _context.WorkTimes
            .Where(x => x.ActualMinutes != x.ExpectedMinutes)
            .Select(x => new { x.ExpectedMinutes, x.ActualMinutes, x.WorkTimeTypeId })
            .ToListAsync();

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
                DurationMinutes = x.ActualMinutes,
                x.Date
            })
            .Union(_context.Absences.Select(x => new
            {
                Type = nameof(DB.Absence),
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
