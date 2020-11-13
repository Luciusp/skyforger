using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace skyforger.models.spells
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