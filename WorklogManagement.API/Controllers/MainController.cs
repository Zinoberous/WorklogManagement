using Microsoft.AspNetCore.Mvc;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;
        private readonly IConfiguration _config;

        public MainController(ILogger<MainController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        [HttpPost("auth")]
        public IActionResult Auth()
        {
            return Ok();
        }
    }
}