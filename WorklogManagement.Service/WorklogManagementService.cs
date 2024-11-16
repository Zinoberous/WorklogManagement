using Microsoft.EntityFrameworkCore;
using WorklogManagement.Data.Context;
using WorklogManagement.Service.Enums;
using WorklogManagement.Service.Models;

namespace WorklogManagement.Service;

public class WorklogManagementService(WorklogManagementContext context) : IWorklogManagementService
{
    private readonly WorklogManagementContext _context = context;

    public async Task<OvertimeInfo> GetOvertimeAsync()
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

        return new()
        {
            TotalSeconds = totalOvertimeSeconds,
            OfficeSeconds = officeOvertimeSeconds,
            MobileSeconds = mobileOvertimeSeconds,
        };
    }
}
