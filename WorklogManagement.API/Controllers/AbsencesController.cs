using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.Data.Context;

namespace WorklogManagement.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AbsencesController(WorklogManagementContext context) : ControllerBase
{
    private readonly WorklogManagementContext _context = context;

    [HttpGet]
    public async Task<List<Absence>> Get(DateOnly from, DateOnly to)
    {
        return await _context.Absences
            .Where(x => x.Date >= from && x.Date <= to)
            .Select(x => Absence.Map(x))
            .ToListAsync();
    }

    [HttpGet("dates")]
    public async Task<List<DateOnly>> GetDatesWithAbsences()
    {
        var dates = await _context.Absences
            .Select(x => x.Date)
            .Distinct()
            .ToListAsync();

        return dates;
    }

    [HttpPost]
    public async Task<Absence> Post(Absence absence)
    {
        await absence.SaveAsync(_context);

        return absence;
    }

    [HttpDelete("{id}")]
    public async Task Delete(int id)
    {
        await Absence.DeleteAsync(_context, id);
    }
}
