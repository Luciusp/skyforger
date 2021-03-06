using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using skyforger.models;
using skyforger.Utilities;

namespace skyforger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PortalSync : ControllerBase
    {
        private readonly ILogger<PortalSync> _logger;
        private readonly IHttpClientFactory _httpfactory;
        private readonly IConfiguration _config;
        private readonly SpellsContext _sc;

        public PortalSync(ILogger<PortalSync> logger, IHttpClientFactory httpfactory, IConfiguration config,
            SpellsContext sc)
        {
            _logger = logger;
            _httpfactory = httpfactory;
            _config = config;
            _sc = sc;
        }

        //performs full sync of spells
        [HttpGet]
        [Route("")]
        public async Task<IActionResult> FullSync()
        {
            await ScrapeSpells();
            return Ok();
        }

        private async Task ScrapeSpells()
        {
            var errcount = 0;
            using var request = new HttpRequestMessage(HttpMethod.Get,
                $"{_config["ObsidianPortal:BaseURI"]}/" +
                $"{_config["ObsidianPortal:SpellsHub"]}");
            using var client = _httpfactory.CreateClient();

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            //match all uris with *-spells (should be 7)
            var regex = new Regex(@"<li>.*wikis\/.*?-spells");
            
            var match = regex.Matches(content);

            for (int i = 0; i < match.Count; i++)
            {
                var matchendpoint = match[i].Value.Replace("<li><a href=\"/", "");
                using var colorspellhubreq = new HttpRequestMessage(HttpMethod.Get,
                    $"{_config["ObsidianPortal:BaseURI"]}/" +
                    $"{matchendpoint}");
                using var spellhubclient = _httpfactory.CreateClient();

                var spellhubresponse = await spellhubclient.SendAsync(colorspellhubreq);
                var spellhubcontent = await spellhubresponse.Content.ReadAsStringAsync();

                //match all table entries. Start at 1 to skip column titles
                var spellhubregex = new Regex(@"<tr>(.|\n)*?<\/tr>");
                var spellhubtablematch = spellhubregex.Matches(spellhubcontent);

                for (int j = 1; j < spellhubtablematch.Count; j++)
                {
                    var spelltablesplit = spellhubtablematch[j].Value.Split("<td>");
                    var spellurlregex = new Regex("\"(\\/wikis\\/.*?)\"");
                    var spellendpoint = spellurlregex.Match(spelltablesplit[1]).Value.Replace("\"", "");

                    if (spellendpoint == string.Empty)
                        continue;

                    var spelluri = $"{_config["ObsidianPortal:BaseURI"]}{spellendpoint}";

                    var existingentity = _sc.Spells.FirstOrDefault(t => t.SpellUri == spelluri);
                    if (existingentity != null)
                        continue;

                    using var spellreq = new HttpRequestMessage(HttpMethod.Get, spelluri);
                    using var spellclient = _httpfactory.CreateClient();

                    var spellres = await spellclient.SendAsync(spellreq);
                    var spellcontent = await spellres.Content.ReadAsStringAsync();
                    var spellscraperesult = await SpellScraper.TransposeSpell(spellcontent, spelluri);
                    if (spellscraperesult.spell.Valid || !spellscraperesult.errors.Any())
                    {
                        _logger.LogInformation("Adding new spell");
                        _sc.Add(spellscraperesult.spell);

                        await _sc.SaveChangesAsync();
                    }
                    else
                    {
                        _logger.LogError(string.Join(",",spellscraperesult.errors) + $":{spelluri}");
                        errcount++;
                    }
                }
            }
            _logger.LogError($"Operation completed. Total failures: {errcount}");
        }
    }
}