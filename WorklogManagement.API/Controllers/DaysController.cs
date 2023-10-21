using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models;
using WorklogManagement.API.Models.Data;
using WorklogManagement.API.Models.Filter;
using WorklogManagement.DataAccess.Context;
using static WorklogManagement.API.Helper.RequestHelper;
using static WorklogManagement.API.Helper.ReflectionHelper;
using DB = WorklogManagement.DataAccess.Models;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DaysController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public DaysController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var request = await GetBodyAsync<GetRequest<DayFilter>>(HttpContext.Request);

            request ??= new();

            // filter

            var items = _context.Days
                .Where
                (
                    x =>
                    request.Filter == default ||
                    (
                        (request.Filter.IsMobile == null || x.IsMobile == request.Filter.IsMobile) &&
                        (request.Filter.Date == null || x.Date == request.Filter.Date) &&
                        (request.Filter.WorkloadId == null || x.WorkloadId == request.Filter.WorkloadId)
                    )
                );

            var totalItems = items.Count();

            // sorting

            IOrderedQueryable<DB.Day>? orderedItems = null;

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

            GetResult<Day, DayFilter> result = new()
            {
                Sorting = request.Sorting,
                PageSize = request.PageSize,
                Page = request.Page,
                Filter = request.Filter,
                TotalItems = (uint)totalItems,
                TotalPages = request.PageSize == 0 ? 1 : (uint)(totalItems / request.PageSize),
                Data = await page
                    .Select(x => new Day(x))
                    .ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet("single")]
        public async Task<IActionResult> GetSingle()
        {
            var filter = await GetBodyAsync<DayFilter>(HttpContext.Request);

            var day = await _context.Days
                .SingleAsync
                (
                    x =>
                    filter == null ||
                    (
                        (filter.IsMobile == null || x.IsMobile == filter.IsMobile) &&
                        (filter.Date == null || x.Date == filter.Date) &&
                        (filter.WorkloadId == null || x.WorkloadId == filter.WorkloadId)
                    )
                );

            return Ok(new Day(day));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(new Day(await _context.Days.SingleAsync(x => x.Id == id)));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Day day)
        {
            await day.SaveAsync(_context);

            return Ok(day);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var day = await _context.Days.SingleAsync(x => x.Id == id);

            _context.Days.Remove(day);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}