using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Enums;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatisticsController(WorklogManagementContext context) : ControllerBase
    {
        private readonly WorklogManagementContext _context = context;

        [HttpGet("overtime")]
        public async Task<IActionResult> GetOvertimeSeconds()
        {
            var totalOvertimeSeconds = 0;
            var officeOvertimeSeconds = 0;
            var mobileOvertimeSeconds = 0;

            var workTimes = await _context.WorkTimes.ToListAsync();

            Parallel.ForEach(workTimes, entry =>
            {
                var expectedMinutes = entry.ExpectedMinutes;
                var actualMinutes = entry.ActualMinutes;

                var overtimeSeconds = (actualMinutes - expectedMinutes) * 60;

                Interlocked.Add(ref totalOvertimeSeconds, overtimeSeconds);

                if (entry.WorkTimeTypeId == (int)WorkTimeType.Office)
                {
                    Interlocked.Add(ref officeOvertimeSeconds, overtimeSeconds);
                }
                else if (entry.WorkTimeTypeId == (int)WorkTimeType.Mobile)
                {
                    Interlocked.Add(ref mobileOvertimeSeconds, overtimeSeconds);
                }
            });

            var overtime = new
            {
                total = totalOvertimeSeconds,
                office = officeOvertimeSeconds,
                mobile = mobileOvertimeSeconds,
            };

            return Ok(overtime);
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetEntriesSumByType(int? year)
        {
            var totalDays = await _context.WorkTimes
                .Where(x => x.ActualMinutes > 0)
                .Where(x => year == null || x.Date.Year == year)
                .Select(x => x.Date)
                .Distinct()
                .CountAsync();

            var workTimeStatistics = await _context.WorkTimes
                .Where(x => x.ActualMinutes > 0)
                .Where(x => year == null || x.Date.Year == year)
                .GroupBy(x => x.WorkTimeTypeId)
                .ToDictionaryAsync(x => x.Key, x => x.Count());

            var timeCompensationDays = await _context.WorkTimes
                .Where(x => x.ActualMinutes == 0)
                .Where(x => year == null || x.Date.Year == year)
                .CountAsync();

            var absenceStatistics = await _context.Absences
                .Where(x => year == null || x.Date.Year == year)
                .GroupBy(x => x.AbsenceTypeId)
                .ToDictionaryAsync(x => x.Key, x => x.Count());

            var statistics = new
            {
                totalDays,
                officeDays = workTimeStatistics[(int)WorkTimeType.Office],
                mobileDays = workTimeStatistics[(int)WorkTimeType.Mobile],
                timeCompensationsDays = timeCompensationDays,
                holidayDays = absenceStatistics[(int)AbsenceType.Holiday],
                vacationDays = absenceStatistics[(int)AbsenceType.Vacation],
                illDays = absenceStatistics[(int)AbsenceType.Ill],
            };

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
