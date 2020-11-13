using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace skyforger.models.common.Mana
{
    //[JsonConverter(typeof(StringEnumConverter))]
    public class ManaType
    {
        public ManaType()
        {
            
        }
        public ManaType(ManaTypeEnum manatype)
        {
            ManaTypeEnum = manatype;
        }
        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ManaTypeEnum ManaTypeEnum { get; set; }
    }
}