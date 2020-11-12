using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace skyforger.models.common
{
    public class ManaClass
    {
        public ManaClass()
        {
            
        }
        public ManaClass(ManaClassEnum manaclass)
        {
            ManaClassEnum = manaclass;
        }

        public int Id { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ManaClassEnum ManaClassEnum { get; set; }
    }
}