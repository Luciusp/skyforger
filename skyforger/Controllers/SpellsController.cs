using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<ActionResult> GetSpellNames()
        {
            return Ok(_sfc.Spells.Select(t => new {t.Name, t.SpellUri}).ToList());
        }

        [Route("find")]
        public async Task<ActionResult> FindSpellByName()
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
        
        [Route("find/advanced")]
        public async Task<ActionResult> FindSpells(
            string manacolor, 
            string manaclass, 
            string level, 
            string school, 
            string subschool, 
            string descriptor)
        {
            if (string.IsNullOrEmpty(manacolor + manaclass + level + school + subschool + descriptor))
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
                var validint = Int32.TryParse(level, out int intval);
                if (validint)
                {
                    spells = spells.Where(t => t.SpellLevel == intval).ToList();
                }
                else
                {
                    return StatusCode(400, "Invalid level. Make sure it's a number");
                }
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

            return Ok(JsonConvert.SerializeObject(spells));
        }
    }
}