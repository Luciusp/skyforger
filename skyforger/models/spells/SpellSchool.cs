using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellSchool
    {
        public SpellSchool()
        {
            
        }
        public SpellSchool(SpellSchoolEnum spellschoolenum)
        {
            SpellSchoolEnum = spellschoolenum;
        }

        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SpellSchoolEnum SpellSchoolEnum { get; set; }
    }
}