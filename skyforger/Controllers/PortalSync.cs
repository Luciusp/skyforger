using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
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
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/lesser-orb-of-cold";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/gate";
                    var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/clone";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/forced-repentance";

                    using var spellreq = new HttpRequestMessage(HttpMethod.Get, spelluri);
                    using var spellclient = _httpfactory.CreateClient();

                    var spellres = await spellclient.SendAsync(spellreq);
                    var spellcontent = await spellres.Content.ReadAsStringAsync();
                    
                    var test = TransposeSpell(spellcontent, spelluri);
                    Console.WriteLine("done!");
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
            
            //WARNING, multimana spells will show up multiple times on different spell lists. Avoid double counting.
            var spell = new Spell();
            
            //set spell uri
            spell.SpellUri = spelluri;
            
            //set spell name
            var spellnameregex = new Regex(@"<title>.* \| Skies of Glass \| Obsidian Portal<\/title>");
            spell.Name = spellnameregex.Match(spellhtml).Value.Split(" |")[0].Replace("<title>","");

            using var md5 = MD5.Create();
            spell.IdHash = Math.Abs(BitConverter.ToInt32(md5.ComputeHash(Encoding.UTF8.GetBytes(spell.Name)), 0));
            
            //TODO: look for spellid hash for dupes. Log collisions

            /*get entry starting with spellname. Split on <li>, skip the first entry because
             *it's blank and returns -1 indexof
             */
            var spellinforegex = new Regex(@"<div itemprop=\'text\'>(.|\n)*?<\/div>");
            var spellinfo = spellinforegex.Match(spellhtml).Value;

            var spelldetails = spellinfo.Split("<ul>")[1].Split("<li>")
                .Skip(1)
                .Select(t => t.Substring(0,t.IndexOf("</li>", StringComparison.Ordinal))).ToList();
            
            
            //search all spelldetails for school and match against spellschool enum to see if there are any matches
            var schoolinfo = spelldetails.FirstOrDefault(t => t.Split(" ").Any(x => Enum.GetNames(typeof(SpellSchool))
                .ToList().Contains(x)));

            if (schoolinfo == null)
                _logger.LogWarning($"Unable to find school for spell {spelluri}");
            //pattern match: school is not surrounded by special characters, descriptor has
            //brackets and can be list, and subschool is parenthesis CANNOT be a list
            else
            {
                //get stuff surrounded by brackets
                var subschools = Regex.Match(schoolinfo, @"\((.*?)\)").Value;
                
                //get stuff surrounded by parentheses
                var descriptors = Regex.Match(schoolinfo, @"\[(.*?)\]").Value;

                //remove descriptors and subschools from main school
                if (!string.IsNullOrEmpty(subschools))
                {
                    schoolinfo = schoolinfo.Replace(subschools, "");
                    //insert subschools
                    var splitsubschools = subschools.Replace("(", "").Replace(")", "").Split(" or ");
                    foreach (var subsschoolentry in splitsubschools)
                    {
                        spell.SubSchool.Add((SpellSubSchool)Enum.Parse(typeof(SpellSubSchool), subsschoolentry.Trim(), true));
                    }
                }

                if (!string.IsNullOrEmpty(descriptors))
                {
                    schoolinfo = schoolinfo.Replace(descriptors, "");
                    var splitdescriptors = descriptors.Replace("[", "").Replace("]", "").Split(",");
                    //insert descriptors
                    foreach (var descriptorentry in splitdescriptors)
                    {
                        spell.Descriptor.Add((SpellDescriptor)Enum.Parse(typeof(SpellDescriptor), 
                            descriptorentry.Trim().Replace(" ", "_").Replace("-", "_"), true));
                    }
                }
            
                //insert schools
                var schools = schoolinfo.Trim().Split("/");
                foreach (var school in schools)
                {
                    spell.School.Add((SpellSchool)Enum.Parse(typeof(SpellSchool), school, true));
                }

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
                var components = componentinfo.Replace("Components", "").Replace(":", "");
                
                //hand verbal and somatic since they're easy. different approach for material and focus/df
                var componentsplit = components.Split(",");
                for (int i = 0; i < componentsplit.Length; i++)
                {
                    switch (componentsplit[i].ToLower().Trim())
                    {
                        case "v":
                        case "verbal":
                            spell.Components.Add(SpellComponent.Verbal);
                            break;
                        case "s":
                        case "somatic":
                            spell.Components.Add(SpellComponent.Somatic);
                            break;
                    }
                }

                var materialmatches = new Regex(@"[mM][\s]{0,1}\((.*?)\)").Matches(components);
                if (materialmatches.Any())
                {
                    spell.Components.Add(SpellComponent.Material);
                    for (int i = 0; i < materialmatches.Count; i++)
                    {
                        var materialcomponent = materialmatches[i].Value.Replace("M (", "").Replace(")", "");
                        spell.MaterialComponents.Add(char.ToUpper(materialcomponent.First()) + materialcomponent.Substring(1));
                    }
                }
                
                var focusmatches = new Regex(@"([fF]|DF|df|F\/DF|f\/df|DF\/F|df\/f)[\s]{0,1}\((.*?)\)").Matches(components);
                if (focusmatches.Any())
                {
                    for (int i = 0; i < focusmatches.Count; i++)
                    {
                        var focustype = new Regex(@"([fF]|DF|df|F\/DF|f\/df|DF\/F|df\/f)[\s]{0,1}\(").Match(focusmatches[i].Value)
                            .Value.Replace("(", "").Trim().ToLower();
                        switch (focustype)
                        {
                            case "df":
                            case "divinefocus":
                                spell.Components.Add(SpellComponent.Divine_Focus);
                                break;
                            case "f":
                            case "focus":
                                spell.Components.Add(SpellComponent.Focus);
                                break;
                            case "df/f":
                            case "df/focus": 
                            case "divinefocus/f": 
                            case "f/df": 
                            case "focus/df": 
                            case "focus/divinefocus":
                                spell.Components.Add(SpellComponent.Focus);
                                spell.Components.Add(SpellComponent.Divine_Focus);
                                break;
                                
                        }
                        var focus = new Regex(@"\((.*?)\)").Match(focusmatches[i].Value).Value.Replace("(", "").Replace(")", "");
                        if (string.IsNullOrEmpty(focus))
                            _logger.LogWarning($"Spell with Focus is missing Focus description: {spell.SpellUri}");
                        else spell.Focus.Add(char.ToUpper(focus.First()) + focus.Substring(1));
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