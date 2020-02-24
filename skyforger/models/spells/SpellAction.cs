using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using skyforger.models.common;

namespace skyforger.models.spells
{
    public class SpellAction
    {
        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ActionType Type { get; set; }
        public int TimeFactor { get; set; }
    }
}