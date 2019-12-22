using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace skyforger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortalSync : ControllerBase
    {
        private readonly ILogger<PortalSync> _logger;
        private readonly IHttpClientFactory _httpfactory;
        private readonly IConfiguration _config;
        
        public PortalSync(ILogger<PortalSync> logger, IHttpClientFactory httpfactory, IConfiguration config)
        {
            _logger = logger;
            _httpfactory = httpfactory;
            _config = config;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> FullSync()
        {
            await ScrapeSpells();
            return Ok();
        }
        
        private async Task ScrapeSpells()
        {
            var client = _httpfactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, 
                $"{_config["ObsidianPortal:BaseURI"]}/" +
                $"{_config["ObsidianPortal:SpellsHub"]}");
            
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            //match all uris with *-pool-spells (should be 5)
            var regex = new Regex("wikis\\/.*\\-pool\\-spells");
            var match = regex.Matches(content);

            for (int i = 0; i < match.Count; i++)
            {
                using var colorspellhubreq = new HttpRequestMessage(HttpMethod.Get, 
                    $"{_config["ObsidianPortal:BaseURI"]}/" +
                    $"{match[i].Value}");
                var spellhubclient = _httpfactory.CreateClient();

                var spellhubresponse = await spellhubclient.SendAsync(colorspellhubreq);
                var spellhubcontent = await spellhubresponse.Content.ReadAsStringAsync();
                Console.WriteLine("done");
            }
        }
    }
}