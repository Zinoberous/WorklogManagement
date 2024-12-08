using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkTimesController(WorklogManagementContext context) : ControllerBase
{
    private readonly WorklogManagementContext _context = context;

    [HttpGet]
    public async Task<List<WorkTime>> Get(DateOnly from, DateOnly to)
    {
        return await _context.WorkTimes
            .Where(x => x.Date >= from && x.Date <= to)
            .Select(x => WorkTime.Map(x))
            .ToListAsync();
    }

    [HttpGet("dates")]
    public async Task<List<DateOnly>> GetDatesWithWorkTimes()
    {
        var dates = await _context.WorkTimes
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    [HttpPost]
    public async Task<WorkTime> Post(WorkTime workTime)
    {
        await workTime.SaveAsync(_context);

        return workTime;
    }
}
