using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace skyforger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StatusController : ControllerBase
    {
        private readonly ILogger<StatusController> _logger;

        public StatusController(ILogger<StatusController> logger)
        {
            _logger = logger;
        }
        
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Status()
        {
            _logger.LogInformation("Status: OK");
            return await Task.FromResult(Ok());
        }
    }
}