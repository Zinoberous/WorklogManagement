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
        public async Task<IActionResult> Get([FromBody] GetRequest<TicketCommentFilter>? request)
        {
            request ??= new();

            // filter

            var items = _context.TicketComments
                .Where
                (
                    x =>
                    request.Filter == default ||
                    (
                        (request.Filter.TicketId == null || x.TicketId == request.Filter.TicketId)
                    )
                );

            var totalItems = items.Count();

            // sorting

            IOrderedQueryable<DB.TicketComment>? orderedItems = null;

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

            GetResult<TicketComment, TicketCommentFilter> result = new()
            {
                Sorting = request.Sorting,
                PageSize = request.PageSize,
                Page = request.Page,
                Filter = request.Filter,
                TotalItems = (uint)totalItems,
                TotalPages = request.PageSize == 0 ? 1 : (uint)(totalItems / request.PageSize),
                Data = await page
                    .Select(x => new TicketComment(x))
                    .ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet("single")]
        public async Task<IActionResult> Get([FromBody] TicketCommentFilter? filter)
        {
            var comment = await _context.TicketComments
                .SingleAsync
                (
                    x =>
                    filter == null ||
                    (
                        (filter.TicketId == null || x.TicketId == filter.TicketId)
                    )
                );

            return Ok(new TicketComment(comment));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
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