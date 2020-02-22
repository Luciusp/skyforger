using System;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.WebEncoders.Testing;
using skyforger.models;

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
                .Include(t => t.MaterialComponents).ToList();
            return Ok(spells.Skip(randspell.Next(0, spells.Count)).Take(1));
        }
    }
}