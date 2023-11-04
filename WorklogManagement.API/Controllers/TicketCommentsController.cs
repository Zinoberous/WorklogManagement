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
    public class TicketCommentsController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public TicketCommentsController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketCommentsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.TicketComments,
                query,
                x => new TicketComment(x),
                x =>
                    (query.TicketId == null || x.TicketId == query.TicketId)
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(new TicketComment(await _context.TicketComments.SingleAsync(x => x.Id == id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TicketComment comment)
        {
            await comment.SaveAsync(_context);

            return Ok(comment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _context.TicketComments.SingleAsync(x => x.Id == id);

            _context.TicketComments.Remove(comment);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}