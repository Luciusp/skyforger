using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace skyforger.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class BackpacksController : Controller
    {
        private readonly ILogger<BackpacksController> _logger;

        public BackpacksController(ILogger<BackpacksController> logger)
        {
            _logger = logger;
        }
        
        // GET
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}