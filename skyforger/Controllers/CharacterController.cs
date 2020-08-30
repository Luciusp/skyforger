using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace skyforger.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CharacterController : Controller
    {
        private readonly ILogger<CharacterController> _logger;

        public CharacterController(ILogger<CharacterController> logger)
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