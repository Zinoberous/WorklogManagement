using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models.Data;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarEntryTypesController(ILogger<CalendarEntryTypesController> logger, IConfiguration config, WorklogManagementContext context) : ControllerBase
    {
        private readonly ILogger<CalendarEntryTypesController> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly WorklogManagementContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var status = await _context.CalendarEntryTypes
                .OrderBy(x => x.Id)
                .Select(x => new CalendarEntryType(x))
                .ToListAsync();

            return Ok(status);
        }
    }
}