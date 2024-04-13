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
    public class WorklogsController(ILogger<WorklogsController> logger, IConfiguration config, WorklogManagementContext context) : ControllerBase
    {
        private readonly ILogger<WorklogsController> _logger = logger;
        private readonly IConfiguration _config = config;
        private readonly WorklogManagementContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] WorklogsQuery query)
        {
            var result = await RequestHelper.GetAsync
            (
                _context.Worklogs
                    .Include(x => x.Ticket)
                    .Include(x => x.WorklogAttachments),
                query,
                x => new Worklog(x),
                x =>
                    (query.Date == null || x.Date == query.Date.Value) &&
                    (query.TicketId == null || x.TicketId == query.TicketId) &&
                     (query.Search == null || x.Ticket.Title.Contains(query.Search) || (!string.IsNullOrWhiteSpace(x.Description) && x.Description.Contains(query.Search)))
            );

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(new Worklog(await _context.Worklogs.Include(x => x.Ticket).Include(x => x.WorklogAttachments).SingleAsync(x => x.Id == id)));
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
            var worklog = await _context.Worklogs
                .Include(x => x.Ticket)
                .Include(x => x.WorklogAttachments)
                .SingleAsync(x => x.Id == id);

            Parallel.ForEach(worklog.WorklogAttachments, async attachment =>
            {
                await WorklogAttachment.DeleteAsync(_context, attachment.Id);
            });

            _context.Worklogs.Remove(worklog);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}