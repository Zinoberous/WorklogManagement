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
    public class TicketsController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public TicketsController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.Tickets
                    .Include(x => x.TicketStatusLogs)
                    .Include(x => x.TicketAttachments),
                query,
                x => new Ticket(x),
                x =>
                    (query.RefId == null || x.RefId == query.RefId) &&
                    (query.Title == null || x.Title.Contains(query.Title)) &&
                    (query.StatusEnum == null || query.StatusEnum.Contains((Enums.TicketStatus)x.TicketStatusId))
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _context.Tickets
                .Include(x => x.TicketStatusLogs)
                .Include(x => x.TicketAttachments)
                .Include(x => x.Worklogs)
                .SingleAsync(x => x.Id == id);

            return Ok(new Ticket(ticket));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Ticket ticket)
        {
            await ticket.SaveAsync(_context);

            return Ok(ticket);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ticket = await _context.Tickets.SingleAsync(x => x.Id == id);

            _context.Tickets.Remove(ticket);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}