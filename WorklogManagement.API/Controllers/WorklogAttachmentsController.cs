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
    public class WorklogAttachmentsController(ILogger<WorklogAttachmentsController> logger, IConfiguration config, WorklogManagementContext context) : ControllerBase
    {
        private readonly ILogger<WorklogAttachmentsController> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly WorklogManagementContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] WorklogAttachmentsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.WorklogAttachments,
                query,
                x => new WorklogAttachment(x),
                x =>
                    query.WorklogId == null || x.WorklogId == query.WorklogId
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var attachment = await _context.WorklogAttachments
                .Include(x => x.Worklog)
                .SingleAsync(x => x.Id == id);

            return Ok(new WorklogAttachment(attachment));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] WorklogAttachment attachment)
        {
            try
            {
                await attachment.SaveAsync(_context);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

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