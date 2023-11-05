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
    public class WorklogAttachmentsController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public WorklogAttachmentsController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] WorklogAttachmentsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.WorklogAttachments,
                query,
                x => new WorklogAttachment(x),
                x =>
                    (query.WorklogId == null || x.WorklogId == query.WorklogId) &&
                    (query.Name == null || x.Name.Contains(query.Name))
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var attachment = await _context.WorklogAttachments
                .Include(x => x.Worklog)
                .ThenInclude(x => x.Day)
                .SingleAsync(x => x.Id == id);

            return Ok(new WorklogAttachment(attachment));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WorklogAttachment attachment)
        {
            await attachment.SaveAsync(_context);

            return Ok(attachment);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await WorklogAttachment.DeleteAsync(_context, id);

            return Ok();
        }
    }
}