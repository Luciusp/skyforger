using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using skyforger.models;
using skyforger.models.common;
using skyforger.models.spells;
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
        private List<Error> _errors = new List<Error>();
        private readonly SkyforgerContext _sfc;

        public PortalSync(ILogger<PortalSync> logger, IHttpClientFactory httpfactory, IConfiguration config,
            SkyforgerContext sfc)
        {
            _logger = logger;
            _httpfactory = httpfactory;
            _config = config;
            _sfc = sfc;
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
                    
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/sign";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/detect-fire";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/lesser-orb-of-cold";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/gate";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/clone";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/forced-repentance";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/martyrs-last-blessing";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/murderous-command";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/lightning-ring";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/enhance-familiar";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/lookingglass";
                    
                    var existingentity = _sfc.Spells.FirstOrDefault(t => t.SpellUri == spelluri);
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
                        _sfc.Add(spellscraperesult.spell);

                        await _sfc.SaveChangesAsync();
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