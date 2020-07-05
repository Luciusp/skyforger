using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders.Testing;
using Newtonsoft.Json;
using skyforger.models;
using skyforger.models.common;
using skyforger.models.spells;

namespace skyforger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpellsController : ControllerBase
    {
        private readonly ILogger<SpellsController> _logger;
        private readonly SkyforgerContext _sfc;
        
        public SpellsController(ILogger<SpellsController> logger, SkyforgerContext sfc)
        {
            _logger = logger;
            _sfc = sfc;
        }
        
        [Route("random")]
        public async Task<ActionResult> RandomSpell()
        {
            var randspell = new Random();
            var spells = _sfc.Spells
                .Include(t => t.Action)
                .Include(t => t.Components)
                .Include(t => t.Descriptor)
                .Include(t => t.Focus)
                .Include(t => t.Mana)
                .Include(t => t.School)
                .Include(t => t.ManaClass)
                .Include(t => t.SubSchool)
                .Include(t => t.MaterialComponents)
                .ToList();
            return Ok(spells.Skip(randspell.Next(0, spells.Count)).Take(1));
        }

        [Route("names")]
        public async Task<IActionResult> GetSpellNames()
        {
            return Ok(_sfc.Spells.Select(t => new {t.Name, t.SpellUri}).ToList());
        }

        [Route("find")]
        public async Task<IActionResult> FindSpellByName()
        {
            var spells = _sfc.Spells
                .Include(t => t.Action)
                .Include(t => t.Components)
                .Include(t => t.Descriptor)
                .Include(t => t.Focus)
                .Include(t => t.Mana)
                .Include(t => t.School)
                .Include(t => t.ManaClass)
                .Include(t => t.SubSchool)
                .Include(t => t.MaterialComponents)
                .ToList();

            
            return Ok();
        }

        [Route("test")]
        public async Task<IActionResult> Test(string query)
        {
            var spells = _sfc.Spells
                .Include(t => t.Action)
                .Include(t => t.Components)
                .Include(t => t.Descriptor)
                .Include(t => t.Focus)
                .Include(t => t.Mana)
                .Include(t => t.School)
                .Include(t => t.ManaClass)
                .Include(t => t.SubSchool)
                .Include(t => t.MaterialComponents);

            var help = spells.Where(t => t.Mana.Any(v => v.ManaTypeEnum == ManaTypeEnum.Red) && t.Mana.Any(t => t.ManaTypeEnum == ManaTypeEnum.Blue) && !t.Mana.Any(v => v.ManaTypeEnum == ManaTypeEnum.Black));
            var test = spells.Where($"Mana.Any(ManaTypeEnum == ManaTypeEnum.{query} && ManaTypeEnum != ManaTypeEnum.Black)", 1).ToList();
            return Ok(JsonConvert.SerializeObject(help));
        }
        
        [Route("find/advanced")]
        public async Task<IActionResult> FindSpells(
            string manacolor, 
            string manaclass, 
            string level, 
            string school, 
            string subschool, 
            string descriptor,
            string bytext,
            string bytitle,
            string fuzzymatch,
            string random
            )
        {
            if (string.IsNullOrEmpty(manacolor + manaclass + level + school + subschool + descriptor + bytext + bytitle + random))
            {
                return StatusCode(400, "Invalid query");
            }
            
            var spells = _sfc.Spells
                .Include(t => t.Action)
                .Include(t => t.Components)
                .Include(t => t.Descriptor)
                .Include(t => t.Focus)
                .Include(t => t.Mana)
                .Include(t => t.School)
                .Include(t => t.ManaClass)
                .Include(t => t.SubSchool)
                .Include(t => t.MaterialComponents)
                .ToList();

            if (!string.IsNullOrEmpty(manacolor) && manacolor.ToLower() != "none")
            {
                var validcolor = Enum.TryParse(manacolor, true, out ManaTypeEnum parsedmanacolor);
                if (validcolor)
                {
                    spells = spells.Where(t => t.Mana.Any(v => v.ManaTypeEnum == parsedmanacolor)).ToList();
                }
                else
                {
                    return StatusCode(400, "Invalid mana color");
                }
            }

            if (!string.IsNullOrEmpty(manaclass) && manaclass.ToLower() != "none")
            {
                var validclass = Enum.TryParse(manaclass, true, out ManaClassEnum parsedmanaclass);
                if (validclass)
                {
                    spells = spells.Where(t => t.ManaClass.Any(v => v.ManaClassEnum == parsedmanaclass)).ToList();
                }
                else
                {
                    return StatusCode(400, "Invalid mana class");
                }
            }
            
            if (!string.IsNullOrEmpty(level) && level.ToLower() != "any")
            {
                var levelsplit = level.Split("-");
                var validints = new List<int>();
                foreach (var levelcheck in levelsplit)
                {
                    if (Int32.TryParse(levelcheck, out int intval))
                    {
                        validints.Add(intval);
                    }
                    else
                    {
                        return StatusCode(400, "Invalid level. Make sure it's a number");
                    }
                }
                validints.Sort();
                var levelrange = new List<int>();
                if (validints.Count < 2)
                    levelrange.Add(validints.First());
                else
                {
                    levelrange = Enumerable.Range(validints[0], validints[1] - validints[0] +1).ToList();
                }
                
                spells = spells.Where(t =>levelrange.Contains(t.SpellLevel)).ToList();
            }
            
            if (!string.IsNullOrEmpty(school) && school.ToLower() != "none")
            {
                var validschool = Enum.TryParse(school, true, out SpellSchoolEnum parsedschool);
                if (validschool)
                {
                    spells = spells.Where(t => t.School.Any(v => v.SpellSchoolEnum == parsedschool)).ToList();
                }
                else
                {
                    return StatusCode(400, "Invalid school");
                }
            }
            
            if (!string.IsNullOrEmpty(subschool) && subschool.ToLower() != "none")
            {
                var validsubschool = Enum.TryParse(subschool, true, out SpellSubSchoolEnum parsedsubschool);
                if (validsubschool)
                {
                    spells = spells.Where(t => t.SubSchool.Any(v => v.SpellSubSchoolEnum == parsedsubschool)).ToList();
                }
                else
                {
                    return StatusCode(400, "Invalid subschool");
                }
            }
            
            if (!string.IsNullOrEmpty(descriptor) && descriptor.ToLower() != "none")
            {
                var validdescriptor = Enum.TryParse(descriptor, true, out SpellDescriptorEnum parseddescriptor);
                if (validdescriptor)
                {
                    spells = spells.Where(t => t.Descriptor.Any(v => v.SpellDescriptorEnum == parseddescriptor)).ToList();
                }
                else
                {
                    return StatusCode(400, "Invalid descriptor");
                }
            }

            if (!string.IsNullOrEmpty(bytext) && bytext.ToLower() != "any")
            {
                var validfuzzy = bool.TryParse(fuzzymatch, out bool fuzzy);
                if (validfuzzy && fuzzy)
                {
                    bytext = bytext.ToLower();
                }
                else bytext = $" {bytext.ToLower()} ";
                
                spells = spells.Where(t => t.Description.ToLower().Contains(bytext)).ToList();
            }
            
            if (!string.IsNullOrEmpty(bytitle) && bytitle.ToLower() != "any")
            {
                bytitle = bytitle.ToLower();
                
                spells = spells.Where(t => t.Name.ToLower().Contains(bytitle)).ToList();
            }

            if (!string.IsNullOrEmpty(random))
            {
                var takecount = 20;
                var validbool = bool.TryParse(random, out bool result);
                if (validbool && result)
                {
                    if (spells.Count > takecount)
                    {
                        var randspell = new Random();
                        spells = spells.Skip(randspell.Next(0, spells.Count)).Take(takecount).ToList();
                    }
                }
            }
            
            var colorcounts = new Dictionary<string, int>();
            foreach (var color in Enum.GetValues(typeof(ManaTypeEnum)))
            {
                if (color.ToString() == "See_Text")
                    continue;
                colorcounts.Add(color.ToString(), spells.Count(t => t.Mana.Any(v => v.ManaTypeEnum == Enum.Parse<ManaTypeEnum>(color.ToString()))));
            }

            var objres = new SpellQueryResult(colorcounts, spells);
            return Ok(JsonConvert.SerializeObject(objres));
        }
    }
}