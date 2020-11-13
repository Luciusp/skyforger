using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace skyforger.models.spells
{
    public class SpellSubSchool
    {
        public SpellSubSchool()
        {
            
        }
        public SpellSubSchool(SpellSubSchoolEnum spellsubschoolenum)
        {
            SpellSubSchoolEnum = spellsubschoolenum;
        }
        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SpellSubSchoolEnum SpellSubSchoolEnum { get; set; }
    }
}