using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Globalization;
using WorklogManagement.API.Enums;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController(ILogger<StatisticsController> logger, IConfiguration config, WorklogManagementContext context) : ControllerBase
    {
        private readonly ILogger<StatisticsController> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly WorklogManagementContext _context = context;

        [HttpGet("overtime")]
        public async Task<IActionResult> GetOvertimeSeconds()
        {
            var totalOvertimeSeconds = 0;
            var officeOvertimeSeconds = 0;
            var mobileOvertimeSeconds = 0;

            ConcurrentDictionary<DateOnly, int> unknownOvertimes = new();

            var relevantTypes = new[] { CalendarEntryType.WorkTime, CalendarEntryType.Office, CalendarEntryType.Mobile };

            var days =  await _context.CalendarEntries
                .Where(x => relevantTypes.Contains((CalendarEntryType)x.CalendarEntryTypeId))
                .GroupBy(x => x.Date)
                .ToDictionaryAsync(x => x.Key, x => x.ToList());

            var officeDays = days
                .Select(x => x.Value)
                .Where(x =>
                    x.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.Office]) ||
                    x.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.WorkTime, CalendarEntryType.Office]) ||
                    x.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.Office, CalendarEntryType.Mobile]))
                .SelectMany(x => x);

            var totalOfficeWorktime = officeDays
                .Where(x => x.CalendarEntryTypeId == (int)CalendarEntryType.Office)
                .Sum(x => x.DurationSeconds);

            var requiredOfficeWorktime = officeDays
                .Where(x => x.CalendarEntryTypeId == (int)CalendarEntryType.WorkTime)
                .Sum(x => x.DurationSeconds);

            officeOvertimeSeconds = totalOfficeWorktime - requiredOfficeWorktime;

            totalOvertimeSeconds = officeOvertimeSeconds;

            var mobileDays = days
                .Select(x => x.Value)
                .Where(x =>
                    x.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.Mobile]) ||
                    x.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.WorkTime, CalendarEntryType.Mobile]) ||
                    x.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.Office, CalendarEntryType.Mobile]))
                .SelectMany(x => x);

            var totalMobileWorktime = mobileDays
                .Where(x => x.CalendarEntryTypeId == (int)CalendarEntryType.Mobile)
                .Sum(x => x.DurationSeconds);

            var requiredMobileWorktime = mobileDays
                .Where(x => x.CalendarEntryTypeId == (int)CalendarEntryType.WorkTime)
                .Sum(x => x.DurationSeconds);

            mobileOvertimeSeconds = totalMobileWorktime - requiredMobileWorktime;

            totalOvertimeSeconds += mobileOvertimeSeconds;

            var unsureDays = days
                .Where(x =>
                    x.Value.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.WorkTime]) ||
                    x.Value.Select(x => (CalendarEntryType)x.CalendarEntryTypeId).Order().SequenceEqual([CalendarEntryType.WorkTime, CalendarEntryType.Office, CalendarEntryType.Mobile]));

            static int SecondsFromNote(string note)
            {
                // Format: <Titel>: HH:mm | ... => Titel = Büro, Mobil oder Über / HH:mm = timeStr / | ... = optionale Zusatznotiz
                var timeStr = string.Join(':', note.Split('|')[0].Split(':').Skip(1)).Trim();
                return (int)TimeSpan.Parse(timeStr, CultureInfo.InvariantCulture).TotalSeconds;
            }

            Parallel.ForEach(unsureDays, day =>
            {
                var date = day.Key;
                var entries = day.Value;

                var worktimeSeconds = entries.SingleOrDefault(x => (CalendarEntryType)x.CalendarEntryTypeId == CalendarEntryType.WorkTime)?.DurationSeconds ?? 0;
                var officeSeconds = entries.SingleOrDefault(x => (CalendarEntryType)x.CalendarEntryTypeId == CalendarEntryType.Office)?.DurationSeconds ?? 0;
                var mobileSeconds = entries.SingleOrDefault(x => (CalendarEntryType)x.CalendarEntryTypeId == CalendarEntryType.Mobile)?.DurationSeconds ?? 0;

                var overtimeSeconds = (officeSeconds + mobileSeconds) - worktimeSeconds;

                if (overtimeSeconds == 0)
                {
                    return;
                }

                Interlocked.Add(ref totalOvertimeSeconds, overtimeSeconds);

                // Minusstunden
                if (overtimeSeconds < 0)
                {
                    var worktimeNote = entries.Single(x => (CalendarEntryType)x.CalendarEntryTypeId == CalendarEntryType.WorkTime).Note;

                    if (!string.IsNullOrWhiteSpace(worktimeNote) && worktimeNote.StartsWith("Büro:"))
                    {
                        var specifiedSeconds = SecondsFromNote(worktimeNote);

                        Interlocked.Add(ref officeOvertimeSeconds, -specifiedSeconds);
                        Interlocked.Add(ref mobileOvertimeSeconds, -(Math.Abs(overtimeSeconds) - specifiedSeconds));
                    }
                    else if (!string.IsNullOrWhiteSpace(worktimeNote) && worktimeNote.StartsWith("Mobil:"))
                    {
                        var specifiedSeconds = SecondsFromNote(worktimeNote);

                        Interlocked.Add(ref mobileOvertimeSeconds, -specifiedSeconds);
                        Interlocked.Add(ref officeOvertimeSeconds, -(Math.Abs(overtimeSeconds) - specifiedSeconds));
                    }
                    else
                    {
                        unknownOvertimes.TryAdd(date, overtimeSeconds);
                    }
                }
                // Überstunden
                else if (overtimeSeconds > 0)
                {
                    var officeNote = entries.Single(x => (CalendarEntryType)x.CalendarEntryTypeId == CalendarEntryType.Office).Note;

                    if (!string.IsNullOrWhiteSpace(officeNote) && officeNote.StartsWith("Über:"))
                    {
                        var specifiedSeconds = SecondsFromNote(officeNote);

                        Interlocked.Add(ref officeOvertimeSeconds, specifiedSeconds);

                        overtimeSeconds -= specifiedSeconds;
                    }

                    var mobileNote = entries.Single(x => (CalendarEntryType)x.CalendarEntryTypeId == CalendarEntryType.Mobile).Note;

                    if (!string.IsNullOrWhiteSpace(mobileNote) && mobileNote.StartsWith("Über:"))
                    {
                        var specifiedSeconds = SecondsFromNote(mobileNote);

                        Interlocked.Add(ref mobileOvertimeSeconds, specifiedSeconds);

                        overtimeSeconds -= specifiedSeconds;
                    }

                    if (overtimeSeconds != 0)
                    {
                        unknownOvertimes.TryAdd(date, overtimeSeconds);
                    }
                }
            });

            var overtime = new
            {
                total = totalOvertimeSeconds,
                office = officeOvertimeSeconds,
                mobile = mobileOvertimeSeconds,
                unknown = unknownOvertimes
            };

            return Ok(overtime);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetCalendarEntriesSumByType(int? year)
        {
            var statistics = await _context.CalendarEntries
                .Where(x => year == null || x.Date.Year == year)
                .GroupBy(x => x.CalendarEntryTypeId)
                .ToDictionaryAsync(x => x.Key, x => x.Count());

            return Ok(statistics);
        }

        [HttpGet("tickets")]
        public async Task<IActionResult> GetTicketsSumByStatus()
        {
            var statistics = await _context.Tickets
                .GroupBy(x => x.TicketStatusId)
                .ToDictionaryAsync(x => x.Key, x => x.Count());

            return Ok(statistics);
        }
    }
}
