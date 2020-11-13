using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace skyforger.models.spells
{
    public class SpellDescriptor
    {
        public SpellDescriptor()
        {
            
        }
        public SpellDescriptor(SpellDescriptorEnum spelldescriptorenum)
        {
            SpellDescriptorEnum = spelldescriptorenum;
        }

        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SpellDescriptorEnum SpellDescriptorEnum { get; set; }
    }
}