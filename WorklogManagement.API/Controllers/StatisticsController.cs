using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var requiredWorktime = await _context.CalendarEntries
                .Where(x => x.CalendarEntryTypeId == (int)CalendarEntryType.WorkTime)
                .SumAsync(x => x.DurationSeconds);

            var totalWorktime = await _context.CalendarEntries
                .Where(x => x.CalendarEntryTypeId == (int)CalendarEntryType.Office || x.CalendarEntryTypeId == (int)CalendarEntryType.Mobile)
                .SumAsync(x => x.DurationSeconds);

            var overtime = totalWorktime - requiredWorktime;

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
