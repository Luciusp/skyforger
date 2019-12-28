using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using skyforger.models;
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
        private List<Error> _errors;

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
                    
                    //var spelluri = $"{_config["ObsidianPortal:BaseURI"]}{spellendpoint}";
                    var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/lesser-orb-of-cold";

                    using var spellreq = new HttpRequestMessage(HttpMethod.Get, spelluri);
                    using var spellclient = _httpfactory.CreateClient();

                    var spellres = await spellclient.SendAsync(spellreq);
                    var spellcontent = await spellres.Content.ReadAsStringAsync();
                    
                    var test = TransposeSpell(spellcontent, spelluri);
                }
            }
        }

        private Spell TransposeSpell(string spellhtml, string spelluri)
        {
            
            /*
             * We're pulling out something that looks like this:
             *  <div itemprop='text'>
                <strong>Burning Ember</strong>
	                <ul>
	                <li>Evocation [fire]</li>
		                <li>Level: Red 0</li>
		                <li>Components: V, S</li>
		                <li>Casting Time: 1 Standard Action</li>
		                <li>Range: Close (25 ft. + 5 ft./2 levels)</li>
		                <li>Effect: One burning ember</li>
		                <li>Duration: Instantaneous</li>
		                <li>Saving Throw: None</li>
		                <li>Spell Resistance: Yes</li>
	                </ul>


	                <p>A small burning ember launches from your pointing finger. You must succeed on a ranged touch attack with the ember to deal damage to a target. The ember deals 1d3 points of fire damage. This spell cannot catch objects on fire.</p>
                </div>
             */
            
            var spell = new Spell();
            
            //set spell uri
            spell.SpellUri = spelluri;
            
            //set spell name
            var spellnameregex = new Regex(@"<title>.* \| Skies of Glass \| Obsidian Portal<\/title>");
            spell.Name = spellnameregex.Match(spellhtml).Value.Split(" |")[0].Replace("<title>","");
            
            /*get entry starting with spellname. Split on <li>, skip the first entry because
             *it's blank and returns -1 indexof
             */
            var spellinforegex = new Regex(@"<div itemprop=\'text\'>(.|\n)*?<\/div>");
            var spellinfo = spellinforegex.Match(spellhtml).Value;

            var spelldetails = spellinfo.Split("<ul>")[1].Split("<li>")
                .Skip(1)
                .Select(t => t.Substring(0,t.IndexOf("</li>", StringComparison.Ordinal))).ToList();
            
            
            //search all spelldetails for school and match it
            var schoolinfo = spelldetails.FirstOrDefault(t => t.Split(" ").Any(x => Enum.GetNames(typeof(SpellSchool))
                .ToList().Contains(x)));

            if (schoolinfo == null)
                _logger.LogWarning($"Unable to find school for spell {spelluri}");
            else
            {
                //pattern match: school is not surrounded by special characters, descriptor has
                //brackets and can be list because fuck you luke, and subschool is parenthesis CANNOT be a list

                var subschools = Regex.Matches(schoolinfo, @"\((.*?)\)").Select(t => t.Value).ToList();
                var descriptors = Regex.Matches(schoolinfo, @"\[(.*?)\]").Select(t => t.Value).ToList();
                var removewords = subschools.Concat(descriptors).ToList();
                
                foreach (var keyword in removewords)
                {
                    schoolinfo = schoolinfo.Replace(keyword, "").Trim();
                }
                spell.School.Add((SpellSchool)Enum.Parse(typeof(SpellSchool), schoolinfo, true));
                
                try
                {
                    var descriptorexists = Enum.TryParse(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(
                        Regex.Replace(schoolinfo.Split(" ")[1], "(?:[^a-zA-Z0-9 ]|(?<=['\"])s)", 
                            string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | 
                                          RegexOptions.Compiled)), out SpellDescriptor descriptor);
//                    if (descriptorexists)
//                        spell.Descriptor = descriptor;
                    
                    //var subschoolexists
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex.Message);
                }
                
            }
            
            //Components. Can be divine focus/focus. See smn monstr 1
            var componentinfo = spelldetails.FirstOrDefault(t => t.Contains("Components"));
            if (componentinfo == null)
                _logger.LogWarning($"Unable to find spell components for {spelluri}");
            else
            {
                var components = componentinfo.Replace("Components","").Replace(":", "")
                    .Split(",");
                for (int i = 0; i < components.Length; i++)
                {
                    var test = components[i].ToLower();
                    switch (components[i].ToLower().Trim())
                    {
                        case "v":
                        case "verbal":
                            spell.Components.Add(SpellComponent.Verbal);
                            break;
                        case "s":
                        case "somatic":
                            spell.Components.Add(SpellComponent.Somatic);
                            break;
                        case "m":
                        case "material":
                            spell.Components.Add(SpellComponent.Material);
                            break;
                    }
                }
            }

            //Mana. Diverse: Comma separated, Multimana: Slash separated last character will always be spell level
            //examples: Diverse: smn mnstr 1, Multi: Planeshift, Multi + Diverse: Divine Power, Prismatic hell: Secret Chest
            //Pattern: Regex * to number, split accordingly. Divide and conquer
            var manainfo = spelldetails.FirstOrDefault(t => t.Contains("Level"));
            if (manainfo == null)
                _logger.LogWarning($"Mana information malformed for spell {spelluri}");
            else
            {
                var manatypes = manainfo.Split(" ").Where(t => Enum.GetNames(typeof(ManaType)).Contains(t.Trim())).ToList();
                if (manatypes == null)
                    _logger.LogWarning($"Mana type missing for spell {spelluri}");
                else
                {
                    foreach (var manatype in manatypes)
                    {
                        var manaexists = Enum.TryParse(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(manatype), 
                            out ManaType mana);
                        
                        if (manaexists)
                            spell.Mana.Add(mana);
                        else _logger.LogWarning($"Invalid manatype for spell {spelluri}");
                    }
                }

                //set spell level to something absurd so I can catch ones that don't have a level assigned
                int level = 9000;
                var levelexists = manainfo.Split(" ").Any(t => Int32.TryParse(t, out level));
                if (levelexists)
                    _logger.LogWarning($"Missing level info for spell {spelluri}");
                else spell.SpellLevel = level;
            }
            
            //Action
            
            //Description
            spell.Description = Regex.Replace(spellinfo.Split("<p>")[1], 
                                                "<[^>]*>", string.Empty);
            
            return spell;
        }
    }
}