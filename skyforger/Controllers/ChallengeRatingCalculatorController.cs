using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            _logger.LogInformation("Fetching CR Calc");
            return await Task.FromResult(View());
        }
    }
}