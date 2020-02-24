using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellComponent
    {
        public SpellComponent()
        {
            
        }
        public SpellComponent(SpellComponentEnum spellcomponentenum)
        {
            SpellComponentEnum = spellcomponentenum;
        }

        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SpellComponentEnum SpellComponentEnum { get; set; }
    }
}