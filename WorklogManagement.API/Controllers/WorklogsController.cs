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
        public async Task<IActionResult> Get([FromQuery] WorklogsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.Worklogs,
                query,
                x => new Worklog(x),
                x =>
                    (query.Date == null || x.Date == query.Date) &&
                    (query.TicketId == null || x.TicketId == query.TicketId)
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
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