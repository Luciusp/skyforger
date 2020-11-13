using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using skyforger.models.player;

namespace skyforger.Controllers
{
    public class HomeController : Controller
    {
        private readonly PlayersContext _pc;

        public HomeController(PlayersContext pc)
        {
            _pc = pc;
        }
        
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string authid = User.FindFirstValue(ClaimTypes.NameIdentifier);
                ViewData["playerdata"] =
                    _pc.Players.FirstOrDefault(t => t.Auth0Id == authid);
            }
            return await Task.FromResult(View());
        }
    }
}