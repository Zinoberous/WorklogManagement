using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Helper;
using WorklogManagement.API.Implements;
using WorklogManagement.API.Models.Data;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalendarEntriesController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public CalendarEntriesController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] CalendarEntryQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.CalendarEntries,
                query,
                x => new CalendarEntry(x),
                x =>
                    (query.Date == null || x.Date == query.Date.Value) &&
                    (query.From == null || x.Date >= query.From.Value) &&
                    (query.To == null || x.Date <= query.To.Value) &&
                    (query.CalendarEntryTypeEnum == null || x.CalendarEntryTypeId == (int)query.CalendarEntryTypeEnum)
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var day = await _context.CalendarEntries.SingleOrDefaultAsync(x => x.Id == id);

            if (day is null)
            {
                return NotFound();
            }

            return Ok(new CalendarEntry(day));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CalendarEntry day)
        {
            await day.SaveAsync(_context);

            return Ok(day);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var day = await _context.CalendarEntries.SingleAsync(x => x.Id == id);

            _context.CalendarEntries.Remove(day);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}