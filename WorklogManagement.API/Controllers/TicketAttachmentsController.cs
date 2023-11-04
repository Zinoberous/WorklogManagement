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
    public class TicketAttachmentsController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public TicketAttachmentsController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketAttachmentsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.TicketAttachments,
                query,
                x => new TicketAttachment(x),
                x =>
                    (query.TicketId == null || x.TicketId == query.TicketId) &&
                    (query.Name == null || x.Name.Contains(query.Name, StringComparison.InvariantCultureIgnoreCase))
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(new TicketAttachment(await _context.TicketAttachments.SingleAsync(x => x.Id == id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TicketAttachment attachment)
        {
            await attachment.SaveAsync(_context);

            return Ok(attachment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var attachment = await _context.TicketAttachments.SingleAsync(x => x.Id == id);

            _context.TicketAttachments.Remove(attachment);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}