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
    public class TicketStatusLogsController(ILogger<TicketStatusLogsController> logger, IConfiguration config, WorklogManagementContext context) : ControllerBase
    {
        private readonly ILogger<TicketStatusLogsController> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly WorklogManagementContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketStatusLogQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.TicketStatusLogs,
                query,
                x => new TicketStatusLog(x),
                x =>
                    query.TicketId == null || x.TicketId == query.TicketId
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticketStatusLog = await _context.TicketStatusLogs
                .SingleAsync(x => x.Id == id);

            return Ok(new TicketStatusLog(ticketStatusLog));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TicketStatusLog ticketStatusLog)
        {
            await ticketStatusLog.SaveAsync(_context);

            return Ok(ticketStatusLog);
        }
    }
}