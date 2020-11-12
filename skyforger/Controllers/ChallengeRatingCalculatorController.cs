using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using skyforger.models.player;

namespace skyforger.Controllers
{
    public class ChallengeRatingCalculatorController : Controller
    {
        private readonly ILogger<ChallengeRatingCalculatorController> _logger;

        public ChallengeRatingCalculatorController(ILogger<ChallengeRatingCalculatorController> logger)
        {
            _logger = logger;
        }
        
        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}