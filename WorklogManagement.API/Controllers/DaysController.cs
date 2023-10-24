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
    public class DaysController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public DaysController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] DaysQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.Days,
                query,
                x => new Day(x),
                x =>
                    (query.IsMobile == null || x.IsMobile == query.IsMobile) &&
                    (query.Date == null || x.Date == query.Date) &&
                    (query.WorkloadId == null || x.WorkloadId == query.WorkloadId)
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(new Day(await _context.Days.SingleAsync(x => x.Id == id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Day day)
        {
            await day.SaveAsync(_context);

            return Ok(day);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var day = await _context.Days.SingleAsync(x => x.Id == id);

            _context.Days.Remove(day);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}