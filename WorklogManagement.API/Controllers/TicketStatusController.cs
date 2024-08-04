using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorklogManagement.API.Models.Data;
using WorklogManagement.DataAccess.Context;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TicketStatusController(WorklogManagementContext context) : ControllerBase
    {
        private readonly WorklogManagementContext _context = context;

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var status = await _context.TicketStatuses
                .OrderBy(x => x.Id)
                .Select(x => new TicketStatus(x))
                .ToListAsync();

            return Ok(status);
        }
    }
}