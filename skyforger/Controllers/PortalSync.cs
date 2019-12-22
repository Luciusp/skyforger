using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using skyforger.models.spells;

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
            using var request = new HttpRequestMessage(HttpMethod.Get, 
                $"{_config["ObsidianPortal:BaseURI"]}/" +
                $"{_config["ObsidianPortal:SpellsHub"]}");
            using var client = _httpfactory.CreateClient();
            
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
                using var spellhubclient = _httpfactory.CreateClient();

                var spellhubresponse = await spellhubclient.SendAsync(colorspellhubreq);
                var spellhubcontent = await spellhubresponse.Content.ReadAsStringAsync();

                //match all table entries. Start at 1 to skip column titles
                var spellhubregex = new Regex("<tr>\n\t\t\t<td>.*\n.*\n.*\n.*\n*.\t<\\/tr>");
                var spellhubtablematch = spellhubregex.Matches(spellhubcontent);
                for (int j = 1; j < spellhubtablematch.Count; j++)
                {
                    var spelltablesplit = spellhubtablematch[j].Value.Split("<td>");
                    var spellurlregex = new Regex("\"(\\/wikis\\/.*?)\"");
                    var spellendpoint = spellurlregex.Match(spelltablesplit[1]).Value.Replace("\"", "");

                    if (spellendpoint == string.Empty)
                        continue;
                    
                    //insert joke about being bad at regex
                    var spellurl = $"{_config["ObsidianPortal:BaseURI"]}{spellendpoint}";
                    
                    using var spellreq = new HttpRequestMessage(HttpMethod.Get, spellurl);
                    using var spellclient = _httpfactory.CreateClient();

                    var spellres = await spellclient.SendAsync(spellreq);
                    var spellcontent = await spellres.Content.ReadAsStringAsync();
                    var test = TransposeSpell(spellcontent);
                }
            }
        }

        private Spell TransposeSpell(string spellhtml)
        {
            var spell = new Spell();
            
            var spellnameregex = new Regex(@"<title>.* \| Skies of Glass \| Obsidian Portal<\/title>");
            spell.Name = spellnameregex.Match(spellhtml).Value.Split(" |")[0].Replace("<title>","");
            
            return spell;
        }
    }
}