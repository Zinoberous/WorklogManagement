using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.API.Models.Data;
using WorklogManagement.API.Models.Filter;
using WorklogManagement.DataAccess.Context;
using static WorklogManagement.API.Helper.ReflectionHelper;
using DB = WorklogManagement.DataAccess.Models;

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
        public async Task<IActionResult> Get([FromBody] GetRequest<TicketAttachmentFilter>? request)
        {
            request ??= new();

            // filter

            var items = _context.TicketAttachments
                .Where
                (
                    x =>
                    request.Filter == default ||
                    (
                        (request.Filter.TicketId == null || x.TicketId == request.Filter.TicketId) &&
                        (request.Filter.Name == null || x.Name.Contains(request.Filter.Name, StringComparison.InvariantCultureIgnoreCase))
                    )
                );

            var totalItems = items.Count();

            // sorting

            IOrderedQueryable<DB.TicketAttachment>? orderedItems = null;

            foreach (var sort in request.Sorting)
            {
                if (orderedItems == null)
                {
                    orderedItems = sort.Desc
                        ? items.OrderByDescending(x => GetPropertyValueByName(x, sort.Column))
                        : items.OrderBy(x => GetPropertyValueByName(x, sort.Column));
                }
                else
                {
                    orderedItems = sort.Desc
                        ? orderedItems.ThenByDescending(x => GetPropertyValueByName(x, sort.Column))
                        : orderedItems.ThenBy(x => GetPropertyValueByName(x, sort.Column));
                }
            }

            items = orderedItems ?? items.OrderBy(x => x.Id);

            // paging

            var page = request.PageSize == 0 ? items : items
                .Skip((int)(request.Page * request.PageSize))
                .Take((int)request.PageSize);

            // result

            GetResult<TicketAttachment, TicketAttachmentFilter> result = new()
            {
                Sorting = request.Sorting,
                PageSize = request.PageSize,
                Page = request.Page,
                Filter = request.Filter,
                TotalItems = (uint)totalItems,
                TotalPages = request.PageSize == 0 ? 1 : (uint)(totalItems / request.PageSize),
                Data = await page
                    .Select(x => new TicketAttachment(x))
                    .ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet("single")]
        public async Task<IActionResult> Get([FromBody] TicketAttachmentFilter? filter)
        {
            var attachment = await _context.TicketAttachments
                .SingleAsync
                (
                    x =>
                    filter == null ||
                    (
                        (filter.TicketId == null || x.TicketId == filter.TicketId) &&
                        (filter.Name == null || x.Name.Contains(filter.Name, StringComparison.InvariantCultureIgnoreCase))
                    )
                );

            return Ok(new TicketAttachment(attachment));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
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