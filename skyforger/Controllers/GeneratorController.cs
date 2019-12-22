using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using skyforger.models;
using skyforger.models.creatures;
using skyforger.Utilities;

namespace skyforger.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeneratorController : ControllerBase
    {
        private readonly ILogger<GeneratorController> _logger;

        public GeneratorController(ILogger<GeneratorController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult> Generate([FromBody] JsonElement query)
        {
            try
            {
                var json = JsonConvert.DeserializeObject<GeneratorQuery>(query.ToString());
            }
            catch (JsonSerializationException jse)
            {
                return BadRequest(await ReturnIntelligentErrorMsg(jse));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
            return Ok();
        }

        //parses error message "Error converting value ..." and attempts to make suggestions
        private async Task<string> ReturnIntelligentErrorMsg(JsonSerializationException error)
        {
            //set comparison to 100 and decrease to difference if applicable
            var ldval = 100;
            var suggestedcorrection = string.Empty;
            
            var badvalue = error.Message.Split("value \"")[1].Split("\" to type")[0];
            var badtype = error.Message.Split("to type '")[1].Split("'. Path")[0];
            var shortenedtype = badtype.Split(".")[badtype.Split(".").Length-1];
            
            switch (shortenedtype)
            {
                case "CreatureAlignment":
                    foreach (var alignment in Enum.GetValues(typeof(CreatureAlignment)))
                    {
                        var compute = LevenshteinDistance.Compute(badvalue, alignment.ToString());
                        if (compute < ldval)
                        {
                            ldval = compute;
                            suggestedcorrection = alignment.ToString();
                        }
                    }
                    
                    break;
                case "ManaType":
                    foreach (var manatype in Enum.GetValues(typeof(ManaType)))
                    {
                        var compute = LevenshteinDistance.Compute(badvalue, manatype.ToString());
                        if (compute < ldval)
                        {
                            ldval = compute;
                            suggestedcorrection = manatype.ToString();
                        }
                    }
                    
                    break;
                default:
                    return error.Message;
            }
            
            
            return $"Unable to parse value {badvalue}. Did you mean {suggestedcorrection}?";
        }
    }
}