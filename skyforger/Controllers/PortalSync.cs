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
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using skyforger.models;
using skyforger.models.common;
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
            var spells = await ScrapeSpells();
            System.IO.File.WriteAllText(Environment.CurrentDirectory + "spells.json",JsonConvert.SerializeObject(spells));
            return Ok();
        }

        private async Task<List<Spell>> ScrapeSpells()
        {
            var spelllist = new List<Spell>();
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

                    var spelluri = $"{_config["ObsidianPortal:BaseURI"]}{spellendpoint}";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/lesser-orb-of-cold";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/gate";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wiki_pages/clone";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/forced-repentance";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/martyrs-last-blessing";
                    //var spelluri = "https://skies-of-glass.obsidianportal.com/wikis/murderous-command";

                    using var spellreq = new HttpRequestMessage(HttpMethod.Get, spelluri);
                    using var spellclient = _httpfactory.CreateClient();

                    var spellres = await spellclient.SendAsync(spellreq);
                    var spellcontent = await spellres.Content.ReadAsStringAsync();
                    
                    spelllist.Add(await TransposeSpell(spellcontent, spelluri));
                }
            }

            return spelllist;
        }

        private async Task<Spell> TransposeSpell(string spellhtml, string spelluri)
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
            spell.Name = spellnameregex.Match(spellhtml).Value.Split(" |")[0].Replace("<title>", "");

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
                .Select(t => t.Substring(0, t.IndexOf("</li>", StringComparison.Ordinal))).ToList();


            //search all spelldetails for school and match against spellschool enum to see if there are any matches
//            var schoolinfo = spelldetails.FirstOrDefault(t => t.Split(" ").Any(x => Enum.GetNames(typeof(SpellSchool))
//                .ToList().Contains(x)));

            var schoolpattern = await RegexFromEnum<SpellSchool>();
            var schoolinfo = spelldetails.FirstOrDefault(t => !string.IsNullOrEmpty(Regex.Match(t, schoolpattern).Value));

            if (string.IsNullOrEmpty(schoolinfo))
                _logger.LogError($"Unable to find school for spell {spelluri}");
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
                        spell.SubSchool.Add((SpellSubSchool) Enum.Parse(typeof(SpellSubSchool), subsschoolentry.Trim(),
                            true));
                    }
                }

                if (!string.IsNullOrEmpty(descriptors))
                {
                    schoolinfo = schoolinfo.Replace(descriptors, "");
                    var splitdescriptors = descriptors.Replace("[", "").Replace("]", "").Split(",");
                    //insert descriptors
                    foreach (var descriptorentry in splitdescriptors)
                    {
                        try
                        {
                            spell.Descriptor.Add((SpellDescriptor) Enum.Parse(typeof(SpellDescriptor),
                                descriptorentry.Trim().Replace(" ", "_").Replace("-", "_"), true));
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e,"Unable to parse spell descriptor");
                        }
                    }
                }

                //insert schools
                var schools = schoolinfo.Trim().Split("/");
                foreach (var school in schools)
                {
                    spell.School.Add((SpellSchool) Enum.Parse(typeof(SpellSchool), school, true));
                }
            }

            //Components. Can be divine focus/focus. See smn monstr 1
            var componentinfo = spelldetails.FirstOrDefault(t => t.Contains("Components"));
            if (componentinfo == null)
                _logger.LogError($"Unable to find spell components for {spelluri}");
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
                        spell.MaterialComponents.Add(char.ToUpper(materialcomponent.First()) +
                                                     materialcomponent.Substring(1));
                    }
                }

                var focusmatches =
                    new Regex(@"([fF]|DF|df|F\/DF|f\/df|DF\/F|df\/f)[\s]{0,1}\((.*?)\)").Matches(components);
                if (focusmatches.Any())
                {
                    for (int i = 0; i < focusmatches.Count; i++)
                    {
                        var focustype = new Regex(@"([fF]|DF|df|F\/DF|f\/df|DF\/F|df\/f)[\s]{0,1}\(")
                            .Match(focusmatches[i].Value)
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

                        var focus = new Regex(@"\((.*?)\)").Match(focusmatches[i].Value).Value.Replace("(", "")
                            .Replace(")", "");
                        if (string.IsNullOrEmpty(focus))
                            _logger.LogError($"Spell with Focus is missing Focus description: {spell.SpellUri}");
                        else spell.Focus.Add(char.ToUpper(focus.First()) + focus.Substring(1));
                    }
                }
            }

            //Mana. Diverse: Comma separated, Multimana: Slash separated last character will always be spell level
            //examples: Diverse: smn mnstr 1, Multi: Planeshift, Multi + Diverse: Divine Power, Prismatic hell: Secret Chest
            //Pattern: Regex * to number, split accordingly. Divide and conquer
            var manainfo = spelldetails.FirstOrDefault(t => t.Contains("Level"));
            if (manainfo == null)
                _logger.LogError($"Mana information malformed for spell {spelluri}");
            else
            {
                var tags = new Regex("data-tag=\".*?\"").Matches(spellhtml);
                for (int i = 0; i < tags.Count; i++)
                {
                    var manaexists = Enum.TryParse(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tags[i].Value
                            .Replace("data-tag=", "")
                            .Replace("\"", "").Trim().ToLower()),
                        out ManaType mana);

                    if (manaexists)
                    {
                        spell.Mana.Add(mana);
                        continue;
                    }

                    //some spells are mono + multi or mono + diverse. Mono isn't a tag, so that will need to be calculated elsewhere
                    var othertag = tags[i].Value.Replace("data-tag=", "").Replace("\"", "")
                        .Replace("-", "_").Replace(" ", "_").ToLower();

                    var manaclassexists = Enum.TryParse(othertag, true,
                        out ManaClass manaclass);

                    if (manaclassexists)
                        spell.ManaClass.Add(manaclass);
                }

                //determine if spell has more than one class
                var multimana =
                    new Regex(@"([Rr]ed|[Gg]reen|[Ww]hite|[Bb]lue|[Bb]lack)(\/.*?(?=\s))").Matches(manainfo);
                if (multimana.Any())
                {
                    var temp = manainfo;
                    for (int i = 0; i < multimana.Count; i++)
                    {
                        temp = manainfo.Replace(multimana[i].Value, "");
                    }

                    if (temp.ToLower().Contains("or"))
                    {
                        spell.ManaClass.Add(ManaClass.Mono);
                    }
                }
                if(!spell.ManaClass.Any())
                    spell.ManaClass.Add(ManaClass.Mono);

                //set spell level to something absurd so I can catch ones that don't have a level assigned
                int level = 9000;
                var levelexists = manainfo.Split(" ").Any(t => Int32.TryParse(t, out level));
                if (!levelexists)
                    _logger.LogError($"Missing level info for spell {spelluri}");
                else spell.SpellLevel = level;

                //copy actual mana rawdescription
                spell.ManaDescription =
                    Regex.Replace(manainfo.Replace($" {level}", ""), @"[Ll]evel[:]{0,1}[\s]{0,1}", "");
            }

            //Description
            spell.Description = Regex.Replace(spellinfo.Split("<p>")[1],
                "<[^>]*>", string.Empty);

            //Action
            var actioninfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("casting time"));
            if (!string.IsNullOrEmpty(actioninfo))
            {
                actioninfo = Regex.Replace(actioninfo, @"[Cc]asting[\s]{0,2}[Tt]ime[:]{0,2}[\s]{0,2}", "");
                var pattern = await RegexFromEnum<ActionType>();
                var actionreg = new Regex(pattern);
                var actionmatches = actionreg.Matches(actioninfo);
                
                int anytimefactor = 0;
                var timefactorexists = actioninfo.Split(" ").Any(t => Int32.TryParse(t, out anytimefactor));
                if (!timefactorexists || actioninfo.ToLower() != "see text")
                    _logger.LogError($"Missing timefactor action info for spell {spelluri}");
                
//                foreach (var aelement in actioninfo.Split(" "))
//                {
//                    int numresult = 0;
//                    var isnum = Int32.TryParse(aelement, out numresult);
//                    if (isnum)
//                    {
//                        actioninfo = Regex.Replace(actioninfo,$"{numresult.ToString()}[\\s]{{0,1}}", "");
//                    }
//                        
//                }
                
                for (int i = 0; i < actionmatches.Count; i++)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(actionmatches[i].Value))
                            continue;
                        var extractedaction = actionmatches[i].Value;
                        var timefactormatch = Regex.Match(extractedaction, @"[\d]{0,1}[\s]{0,1}").Value;
                        var timefactor = string.IsNullOrEmpty(timefactormatch) ? 0 : Int32.Parse(timefactormatch);
                        if (timefactor != 0)
                        {
                            extractedaction = Regex.Replace(extractedaction,$"{timefactor.ToString()}[\\s]{{0,1}}", "");
                        }
                        var actionexists = Enum.TryParse(extractedaction.ToLower().Replace("action", "")
                                .Replace("see text", "see_text").Trim(), true,
                            out ActionType actionType);
                        if (actionexists)
                        {
                            var action = new SpellAction()
                            {
                                Type = actionType,
                                TimeFactor = timefactor
                            };
                        
                            spell.Action.Add(action);
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Unable to parse action");
                    }
                    
                } 
            }

            //Duration
            var durationinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("duration"));
            if (!string.IsNullOrEmpty(durationinfo))
            {
                durationinfo = Regex.Replace(durationinfo, @"[Dd]uration[:]{0,2}[\s]{0,2}","");
                spell.Duration = durationinfo;
            }

            //Range
            var rangeinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("range"));
            if (!string.IsNullOrEmpty(rangeinfo))
            {
                rangeinfo = Regex.Replace(rangeinfo, @"[Rr]ange[:]{0,2}[\s]{0,2}","");
                spell.Range = char.ToUpper(rangeinfo.First()) + rangeinfo.Substring(1);
            }

            //Saving Throw
            var savingthrowinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("saving throw"));
            if (!string.IsNullOrEmpty(savingthrowinfo))
            {
                savingthrowinfo = Regex.Replace(savingthrowinfo, @"[Ss]aving[\s]{0,1}[Tt]hrow[:]{0,1}[\s]{0,1}", "");
                spell.SavingThrow = char.ToUpper(savingthrowinfo.First()) + savingthrowinfo.Substring(1);
            }

            //Spell resistance
            var spellresistanceinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("spell resistance"));
            if (!string.IsNullOrEmpty(spellresistanceinfo))
            {
                spellresistanceinfo = Regex.Replace(spellresistanceinfo,
                    @"[Ss]pell[\s]{0,1}[Rr]esistance[:]{0,1}[\s]{0,1}", "");
                spell.SpellResistance = char.ToUpper(spellresistanceinfo.First()) + spellresistanceinfo.Substring(1);
            }

            //Target
            var targetinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("target"));
            if (!string.IsNullOrEmpty(targetinfo))
            {
                targetinfo = Regex.Replace(targetinfo, @"[Tt]arget[\s]{0,1}[:]{0,1}[\s]{0,1}", "");
                spell.Target = char.ToUpper(targetinfo.First()) + targetinfo.Substring(1);
            }

            return spell;
        }

        private async Task<string> RegexFromEnum<T>()
        {
            var result = new StringBuilder();

            foreach (var name in Enum.GetNames(typeof(T)))
            {
                var thisname = name.ToLower().Replace("_", " ");
                switch (name.ToLower())
                {
                    case "swift":
                    case "standard":
                    case "immediate":
                    case "move":
                    case "fullround":
                        thisname = $"{thisname} [Aa]ction";
                        break;
                }
                var firstchar = thisname.First();
                var firstcharcap = char.ToUpper(thisname.First());
                var prepend = $"[{firstcharcap}{firstchar}]";
                if (typeof(T).Name == "ActionType")
                {
                    result.Append(@"[\d]{0,1}[\s]{0,1}");
                }
                
                result.Append($"{prepend}{thisname.Substring(1)}");
                if (name != Enum.GetNames(typeof(T)).Last())
                    result.Append("|");
            }
            
            return result.ToString();
        }
    }
}