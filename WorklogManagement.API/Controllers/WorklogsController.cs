using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models.Data;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WorklogsController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public WorklogsController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        // TODO: sorting, paging, filtering
        public async Task<IActionResult> Get()
        {
            var tickets = await _context.Worklogs
                .Select(x => new Worklog(x))
                .ToListAsync();

            return Ok(tickets);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(new Worklog(await _context.Worklogs.SingleAsync(x => x.Id == id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Worklog worklog)
        {
            await worklog.SaveAsync(_context);

            return Ok(worklog);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var worklog = await _context.Worklogs.SingleAsync(x => x.Id == id);

            _context.Worklogs.Remove(worklog);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}