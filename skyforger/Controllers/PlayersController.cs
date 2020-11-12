using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skyforger.models.player;

namespace skyforger.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class PlayersController : Controller
    {
        private readonly ILogger<PlayersController> _logger;
        private PlayersContext _pc;
        
        public PlayersController(ILogger<PlayersController> logger, PlayersContext pc)
        {
            _logger = logger;
            _pc = pc;
        }
        
        // GET
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        [Route("create")]
        public async Task<IActionResult> CreatePlayer([FromForm] Player player)
        {
            player.Auth0Id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playerexists = _pc.Players.Any(t => t.Auth0Id == player.Auth0Id);
            if (playerexists)
            {
                return Ok("Player already exists!");
            }
            
            await _pc.AddAsync(player);
            await _pc.SaveChangesAsync();
            
            return Redirect("/");
        }
    }
}