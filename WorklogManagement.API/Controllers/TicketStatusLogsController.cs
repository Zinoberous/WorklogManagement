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

        private IQueryable<DataAccess.Models.TicketStatusLog> GetItems(string? expand)
        {
            var data = _context.TicketStatusLogs.AsQueryable();

            if (string.IsNullOrWhiteSpace(expand))
            {
                return data;
            }

            var expands = expand.ToLower().Split(',');

            if (expands.Contains("ticket"))
            {
                data = data.Include(x => x.Ticket);
            }

            return data;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketStatusLogQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                GetItems(query.Expand),
                query,
                x => new TicketStatusLog(x),
                x =>
                    (query.TicketId == null || x.TicketId == query.TicketId) &&
                    (query.BeforeTicketStatusLogId == null || x.Id < query.BeforeTicketStatusLogId)
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id, string? expand)
        {
            var ticketStatusLog = await GetItems(expand)
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