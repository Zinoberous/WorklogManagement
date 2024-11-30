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
}
