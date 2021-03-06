using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using skyforger.models;
using skyforger.models.common;
using skyforger.models.common.Mana;
using skyforger.models.spells;

namespace skyforger.Utilities
{
    public class SpellScraper
    {
        public static async Task<(Spell spell, List<Error> errors)> TransposeSpell(string spellhtml, string spelluri)
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
            var errors = new List<Error>();

            //set spell uri
            spell.SpellUri = spelluri.Replace("wiki_pages", "wikis");

            //set spell name
            var spellnameregex = new Regex(@"<title>.* \| Skies of Glass \| Obsidian Portal<\/title>");
            spell.Name = spellnameregex.Match(spellhtml).Value.Split(" |")[0].Replace("<title>", "");

            /*get entry starting with spellname. Split on <li>, skip the first entry because
             *it's blank and returns -1 indexof
             */
            var spellinforegex = new Regex(@"<div itemprop=\'text\'>(.|\n)*?<\/div>");
            var spellinfo = spellinforegex.Match(spellhtml).Value;
            List<string> spelldetails;
            try
            {
                spelldetails = spellinfo.Split("<ul>")[1].Split("<li>")
                    .Skip(1)
                    .Select(t => t.Substring(0, t.IndexOf("</li>", StringComparison.Ordinal))).ToList();
            }
            catch (Exception e)
            {
                var errlist = new List<string>() {"Invalid html in spell range", e.Message};
                var err = new Error("spell", spelluri, errlist);
                errors.Add(err);
                spell.Valid = false;
                return (spell, errors);
            }

            var schoolpattern = await RegexFromEnum<SpellSchoolEnum>();
            var schoolinfo =
                spelldetails.FirstOrDefault(t => !string.IsNullOrEmpty(Regex.Match(t, schoolpattern).Value));

            if (string.IsNullOrEmpty(schoolinfo))
            {
                var errlist = new List<string>() {"School info is missing"};
                var err = new Error("spell", spelluri, errlist);
                errors.Add(err);
                spell.Valid = false;
                return (spell, errors);
            }
            //pattern match: school is not surrounded by special characters, descriptor has
            //brackets and can be list, and subschool is parenthesis CANNOT be a list
            else
            {
                spell.SchoolRaw = schoolinfo;
                //get stuff surrounded by brackets
                var subschools = Regex.Matches(schoolinfo, @"\((.*?)\)");

                //get stuff surrounded by parentheses
                var descriptors = Regex.Matches(schoolinfo, @"\[(.*?)\]");

                //remove descriptors and subschools from main school
                if (subschools.Any())
                {
                    for (int i = 0; i < subschools.Count; i++)
                    {
                        schoolinfo = schoolinfo.Replace(subschools[i].Value, "");
                        //insert subschools
                        var splitsubschools = subschools[i].Value.Replace("(", "").Replace(")", "").Split(" or ");
                        foreach (var subsschoolentry in splitsubschools)
                        {
                            try
                            {
                                spell.SubSchool.Add(new SpellSubSchool((SpellSubSchoolEnum) Enum.Parse(
                                    typeof(SpellSubSchoolEnum), subsschoolentry.Trim(),
                                    true)));
                            }
                            catch (Exception e)
                            {
                                var errlist = new List<string>() {"Unable to parse subschool", e.Message};
                                var err = new Error("spell", spelluri, errlist);
                                errors.Add(err);
                                spell.Valid = false;
                                return (spell, errors);
                            }
                        }
                    }
                }

                if (descriptors.Any())
                {
                    for (int i = 0; i < descriptors.Count; i++)
                    {
                        schoolinfo = schoolinfo.Replace(descriptors[i].Value, "");
                        var splitdescriptors = descriptors[i].Value.Replace("[", "").Replace("]", "").Split(",");
                        //insert descriptors
                        foreach (var descriptorentry in splitdescriptors)
                        {
                            try
                            {
                                spell.Descriptor.Add(new SpellDescriptor(
                                    Enum.Parse<SpellDescriptorEnum>(
                                        descriptorentry.Trim().Replace(" ", "").Replace("-", ""), true)));
                            }
                            catch (Exception e)
                            {
                                var errlist = new List<string>() {"Unable to parse spell descriptor", e.Message};
                                var err = new Error("spell", spelluri, errlist);
                                errors.Add(err);
                            }
                        }
                    }
                }

                //insert schools
                var schools = schoolinfo.Trim().Split("/");
                foreach (var school in schools)
                {
                    spell.School.Add(
                        new SpellSchool((SpellSchoolEnum) Enum.Parse(typeof(SpellSchoolEnum), school, true)));
                }
            }

            //Components. Can be divine focus/focus. See smn monstr 1
            var componentinfo = spelldetails.FirstOrDefault(t => t.Contains("Components"));
            if (componentinfo == null)
            {
                var errlist = new List<string>() {"Unable to find spell components"};
                var err = new Error("spell", spelluri, errlist);
                errors.Add(err);
                spell.Valid = false;
                return (spell, errors);
            }

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
                            spell.Components.Add(new SpellComponent(SpellComponentEnum.Verbal));
                            break;
                        case "s":
                        case "somatic":
                            spell.Components.Add(new SpellComponent(SpellComponentEnum.Somatic));
                            break;
                    }
                }

                var materialmatches = new Regex(@"[mM][\s]{0,1}\((.*?)\)").Matches(components);
                if (materialmatches.Any())
                {
                    spell.Components.Add(new SpellComponent(SpellComponentEnum.Material));
                    for (int i = 0; i < materialmatches.Count; i++)
                    {
                        var materialcomponent = materialmatches[i].Value.Replace("M (", "").Replace(")", "");
                        spell.MaterialComponents.Add(new MaterialComponent(char.ToUpper(materialcomponent.First()) +
                                                                           materialcomponent.Substring(1)));
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
                                spell.Components.Add(new SpellComponent(SpellComponentEnum.DivineFocus));
                                break;
                            case "f":
                            case "focus":
                                spell.Components.Add(new SpellComponent(SpellComponentEnum.Focus));
                                break;
                            case "df/f":
                            case "df/focus":
                            case "divinefocus/f":
                            case "f/df":
                            case "focus/df":
                            case "focus/divinefocus":
                                spell.Components.Add(new SpellComponent(SpellComponentEnum.Focus));
                                spell.Components.Add(new SpellComponent(SpellComponentEnum.DivineFocus));
                                break;
                        }

                        var focus = new Regex(@"\((.*?)\)").Match(focusmatches[i].Value).Value.Replace("(", "")
                            .Replace(")", "");
                        if (string.IsNullOrEmpty(focus))
                        {
                            var errlist = new List<string>() {"Focus is missing focus description"};
                            var err = new Error("spell", spelluri, errlist);
                            errors.Add(err);
                        }
                        else spell.Focus.Add(new Focus(char.ToUpper(focus.First()) + focus.Substring(1)));
                    }
                }
            }

            //Mana. Diverse: Comma separated, Multimana: Slash separated last character will always be spell level
            //examples: Diverse: smn mnstr 1, Multi: Planeshift, Multi + Diverse: Divine Power, Prismatic hell: Secret Chest
            //Pattern: Regex * to number, split accordingly. Divide and conquer
            var manainfo = spelldetails.FirstOrDefault(t => t.Contains("Level"));
            if (manainfo == null)
            {
                var errlist = new List<string>() {"Mana information malformed"};
                var err = new Error("spell", spelluri, errlist);
                errors.Add(err);
                spell.Valid = false;
                return (spell, errors);
            }
            else
            {
                var tags = new Regex("data-tag=\".*?\"").Matches(spellhtml);
                for (int i = 0; i < tags.Count; i++)
                {
                    var manaexists = Enum.TryParse(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(tags[i].Value
                            .Replace("data-tag=", "")
                            .Replace("\"", "").Trim().ToLower()),
                        out ManaTypeEnum mana);

                    if (manaexists)
                    {
                        spell.Mana.Add(new ManaType(mana));
                        continue;
                    }

                    //some spells are mono + multi or mono + diverse. Mono isn't a tag, so that will need to be calculated elsewhere
                    var othertag = tags[i].Value.Replace("data-tag=", "").Replace("\"", "")
                        .Replace("-", "").Replace(" ", "").ToLower();

                    var manaclassexists = Enum.TryParse(othertag, true,
                        out ManaClassEnum manaclass);

                    if (manaclassexists)
                        spell.ManaClass.Add(new ManaClass(manaclass));
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
                        spell.ManaClass.Add(new ManaClass(ManaClassEnum.Mono));
                    }
                }

                if (!spell.ManaClass.Any())
                    spell.ManaClass.Add(new ManaClass(ManaClassEnum.Mono));

                //set spell level to something absurd so I can catch ones that don't have a level assigned
                int level = 9000;
                var levelexists = manainfo.Split(" ").Any(t => Int32.TryParse(t, out level));
                if (!levelexists)
                {
                    var errlist = new List<string>() {"Missing level info"};
                    var err = new Error("spell", spelluri, errlist);
                    errors.Add(err);
                }
                else spell.SpellLevel = level;

                //copy actual mana rawdescription
                spell.ManaDescription =
                    Regex.Replace(manainfo.Replace($" {level}", ""), @"[Ll]evel[:]{0,1}[\s]{0,1}", "");
            }

            //Description
            var description = Regex.Match(spellinfo, @"<p>(.|\n)*?<\/div>").Value;
            spell.Description = Regex.Replace(description, @"<[^>]*>", "").Replace("\t", "");

            //Action
            var actioninfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("casting time"));
            if (!string.IsNullOrEmpty(actioninfo))
            {
                actioninfo = Regex.Replace(actioninfo, @"[Cc]asting[\s]{0,2}[Tt]ime[:]{0,2}[\s]{0,2}", "");
                var pattern = await RegexFromEnum<ActionType>();
                var actionreg = new Regex(pattern);
                var actionmatches = actionreg.Matches(actioninfo);

                var timefactorexists = actioninfo.Split(" ").Any(t => Int32.TryParse(t, out _));
                if (!timefactorexists && actioninfo.ToLower() != "see text")
                {
                    var errlist = new List<string>() {"Missing timefactor action info"};
                    var err = new Error("spell", spelluri, errlist);
                    errors.Add(err);
                }

                for (int i = 0; i < actionmatches.Count; i++)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(actionmatches[i].Value))
                            continue;
                        var extractedaction = actionmatches[i].Value;
                        var timefactormatch = Regex.Match(extractedaction, @"[\d]{0,1}[\s]{0,1}").Value.Trim();
                        var timefactor = (string.IsNullOrEmpty(timefactormatch) || timefactormatch == "")
                            ? 0
                            : Int32.Parse(timefactormatch);
                        if (timefactor != 0)
                        {
                            extractedaction = Regex.Replace(extractedaction, $"{timefactor.ToString()}[\\s]{{0,1}}",
                                "");
                        }

                        var actionexists = Enum.TryParse(extractedaction.ToLower().Replace(" action", "")
                                .TrimStart().Replace(" ", "_").Trim(), true,
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
                        var errlist = new List<string>() {"Potentially malformed action. Unable to parse", e.Message};
                        var err = new Error("spell", spelluri, errlist);
                        errors.Add(err);
                        spell.Valid = false;
                        return (spell, errors);
                    }
                }
            }

            //Duration
            var durationinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("duration"));
            if (!string.IsNullOrEmpty(durationinfo))
            {
                durationinfo = Regex.Replace(durationinfo, @"[Dd]uration[:]{0,2}[\s]{0,2}", "");
                spell.Duration = durationinfo;
            }

            //Range
            var rangeinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("range"));
            if (!string.IsNullOrEmpty(rangeinfo))
            {
                rangeinfo = Regex.Replace(rangeinfo, @"[Rr]ange[:]{0,2}[\s]{0,2}", "");
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

            //Effect
            var effectinfo = spelldetails.FirstOrDefault(t => t.ToLower().StartsWith("effect"));
            if (!string.IsNullOrEmpty(effectinfo))
            {
                effectinfo = Regex.Replace(effectinfo, @"[Ee]ffect[\s]{0,1}[:]{0,1}[\s]{0,1}", "");
                spell.Effect = char.ToUpper(effectinfo.First()) + effectinfo.Substring(1);
            }

            spell.Valid = true;
            return (spell, errors);
        }

        private static async Task<string> RegexFromEnum<T>()
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
                        thisname = $"{thisname} [Aa]ction";
                        break;
                    case "full_round":
                        thisname = $"full [Rr]ound";
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

            return await Task.FromResult(result.ToString());
        }
    }
}