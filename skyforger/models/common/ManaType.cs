using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using skyforger.models.common;

namespace skyforger.models
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