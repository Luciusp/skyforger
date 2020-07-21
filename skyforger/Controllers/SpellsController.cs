using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using skyforger.models;
using skyforger.models.spells;

namespace skyforger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SpellsController : Controller
    {
        private readonly ILogger<SpellsController> _logger;
        private readonly SpellsContext _sc;
        
        public SpellsController(ILogger<SpellsController> logger, SpellsContext sc)
        {
            _logger = logger;
            _sc = sc;
        }
        
        // GET
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm] SpellSearch searchparams)
        {
            ViewData["spells"] = await FindSpells(searchparams);
            return View("SpellSearchResults");
        }

        private async Task<List<Spell>> FindSpells(SpellSearch searchparams)
        {
            var spells = _sc.Spells
                .Include(t => t.Action)
                .Include(t => t.Components)
                .Include(t => t.Descriptor)
                .Include(t => t.Focus)
                .Include(t => t.Mana)
                .Include(t => t.School)
                .Include(t => t.ManaClass)
                .Include(t => t.SubSchool)
                .Include(t => t.MaterialComponents).ToList();
            
            if (searchparams.ManaColor != null)
            {
                spells = spells.Where(t => t.Mana.Any(v => v.ManaTypeEnum == searchparams.ManaColor)).ToList();
            }

            if (searchparams.ManaClass != null)
            {
                spells = spells.Where(t => t.ManaClass.Any(v => v.ManaClassEnum == searchparams.ManaClass)).ToList();
            }

            if (searchparams.SpellLevelLowerBound != null)
            {
                spells = spells.Where(t => t.SpellLevel > searchparams.SpellLevelLowerBound).ToList();
            }

            if (searchparams.SpellLevelUpperBound != null)
            {
                spells = spells.Where(t => t.SpellLevel < searchparams.SpellLevelUpperBound).ToList();
            }

            if (searchparams.SpellSchool != null)
            {
                spells = spells.Where(t => t.School.Any(v => v.SpellSchoolEnum == searchparams.SpellSchool)).ToList();
            }

            if (searchparams.SpellSubSchool != null)
            {
                spells = spells.Where(t => t.SubSchool.Any(v => v.SpellSubSchoolEnum == searchparams.SpellSubSchool))
                    .ToList();
            }

            if (searchparams.SpellDescriptor != null)
            {
                spells = spells.Where(t =>
                    t.Descriptor.Any(v => v.SpellDescriptorEnum == searchparams.SpellDescriptor)).ToList();
            }

            if (searchparams.TitleContainsWords != null)
            {
                spells = spells.Where(t => t.Name.ToLower().Contains(searchparams.TitleContainsWords.ToLower())).ToList();
            }

            if (searchparams.DescriptionContainsWords != null)
            {
                if (searchparams.FuzzyMatchDescription)
                {
                    spells = spells.Where(t =>
                        t.Description.ToLower().Contains(searchparams.DescriptionContainsWords.ToLower())).ToList();
                }
                else
                {
                    spells = spells.Where(t =>
                        t.Description.ToLower().Contains($" {searchparams.DescriptionContainsWords.ToLower()} ")).ToList();
                }
            }

            return spells.ToList();
        }
    }
}