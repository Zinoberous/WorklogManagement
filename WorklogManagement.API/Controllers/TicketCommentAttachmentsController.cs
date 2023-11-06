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
    public class TicketCommentAttachmentsController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public TicketCommentAttachmentsController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] TicketCommentAttachmentsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.TicketCommentAttachments.Include(x => x.TicketComment),
                query,
                x => new TicketCommentAttachment(x),
                x =>
                    (query.TicketCommentId == null || x.TicketCommentId == query.TicketCommentId) &&
                    (query.Name == null || x.Name.Contains(query.Name))
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(new TicketCommentAttachment(await _context.TicketCommentAttachments.SingleAsync(x => x.Id == id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TicketCommentAttachment attachment)
        {
            await attachment.SaveAsync(_context);

            return Ok(attachment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await TicketCommentAttachment.DeleteAsync(_context, id);

            return Ok();
        }
    }
}