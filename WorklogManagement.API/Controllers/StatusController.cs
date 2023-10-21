using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models.Data;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;
        private readonly WorklogManagementContext _context;

        public StatusController(ILogger<MainController> logger, IConfiguration config, WorklogManagementContext context)
        {
            _logger = logger;
            _config = config;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var status = await _context.Statuses
                .OrderBy(x => x.Id)
                .Select(x => new Status(x))
                .ToListAsync();

            return Ok(status);
        }
    }
}