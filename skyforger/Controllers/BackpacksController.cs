using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skyforger.models.backpacks;
using skyforger.models.player;

namespace skyforger.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class BackpacksController : Controller
    {
        private readonly ILogger<BackpacksController> _logger;
        private PlayersContext _pc;
        private BackpacksContext _bc;
        public BackpacksController(ILogger<BackpacksController> logger, BackpacksContext bc, PlayersContext pc)
        {
            _logger = logger;
            _bc = bc;
            _pc = pc;
        }
        
        // GET
        public async Task<IActionResult> Index()
        {
            var currentplayer = _pc.Players.FirstOrDefault(t => t.Auth0Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["playerdata"] = currentplayer;
            
            if (currentplayer == null)
            {
                return Redirect("/");
            }

            var partybackpack = new Player()
            {
                Username = "Party Backpack",
                CharacterName = "Party Backpack",
                ProfilePictureUri = "https://i.imgur.com/L1zjPZv.png"
            };

            var players = _pc.Players.OrderBy(t => t.CharacterName).ToList();
            //put party backpack first so it's rendered first
            players.Insert(0, partybackpack);
            
            ViewData["Backpack"] = _bc.Backpacks.ToList();
            ViewData["Players"] = players;
            return View();
        }

        [Route("allitems")]
        public async Task<IActionResult> AllItems()
        {
            var partybackpack = new Player()
            {
                Username = "Party Backpack",
                CharacterName = "Party Backpack",
                ProfilePictureUri = "https://i.imgur.com/L1zjPZv.png"
            };

            ViewData["Player"] = partybackpack;
            ViewData["Backpack"] = _bc.Backpacks.ToList();
            return View();
        }

        [Route("items")]
        public async Task<IActionResult> Items(int id)
        {
            ViewData["Player"] = _pc.Players.FirstOrDefault(t => t.Id == id);
            return View();
        }

        [HttpPost]
        [Route("additems")]
        public async Task<IActionResult> AddItems([FromBody] List<BackpackItem> items)
        {
            
            return Ok();
        }
    }
}