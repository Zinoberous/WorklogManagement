using Microsoft.AspNetCore.Mvc;

namespace WorklogManagement.API.Controllers
{
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class MainController(ILogger<MainController> logger, IConfiguration config) : ControllerBase
    {
        private readonly ILogger<MainController> _logger = logger;
        private readonly IConfiguration _config = config;
    }
}