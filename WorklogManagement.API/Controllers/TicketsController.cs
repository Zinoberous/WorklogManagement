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
        public async Task<IActionResult> Get([FromBody] GetRequest<TicketFilter>? request)
        {
            request ??= new();

            // filter

            var items = _context.Tickets
                .Where
                (
                    x =>
                    request.Filter == default ||
                    (
                        (request.Filter.RefId == null || x.RefId == request.Filter.RefId) &&
                        (request.Filter.Title == null || x.Title.Contains(request.Filter.Title, StringComparison.InvariantCultureIgnoreCase)) &&
                        (request.Filter.StatusId == null || x.StatusId == request.Filter.StatusId)
                    )
                );

            var totalItems = items.Count();

            // sorting

            IOrderedQueryable<DB.Ticket>? orderedItems = null;

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

            GetResult<Ticket, TicketFilter> result = new()
            {
                Sorting = request.Sorting,
                PageSize = request.PageSize,
                Page = request.Page,
                Filter = request.Filter,
                TotalItems = (uint)totalItems,
                TotalPages = request.PageSize == 0 ? 1 : (uint)(totalItems / request.PageSize),
                Data = await page
                    .Select(x => new Ticket(x))
                    .ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet("single")]
        public async Task<IActionResult> Get([FromBody] TicketFilter? filter)
        {
            var ticket = await _context.Tickets
                .SingleAsync
                (
                    x =>
                    filter == null ||
                    (
                        (filter.RefId == null || x.RefId == filter.RefId) &&
                        (filter.Title == null || x.Title.Contains(filter.Title, StringComparison.InvariantCultureIgnoreCase)) &&
                        (filter.StatusId == null || x.StatusId == filter.StatusId)
                    )
                );

            return Ok(new Ticket(ticket));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ticket = await _context.Tickets
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