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
        var totalOvertimeMinutes = 0;
        var officeOvertimeMinutes = 0;
        var mobileOvertimeMinutes = 0;

        var workTimes = await _context.WorkTimes.ToListAsync();

        Parallel.ForEach(workTimes, entry =>
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
}
