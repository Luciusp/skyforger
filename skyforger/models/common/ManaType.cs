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
        public ManaType(ManaTypeEnum type)
        {
            ManaTypeEnum = type;
        }
        public int Id { get; set; }
        public ManaTypeEnum ManaTypeEnum { get; set; }
    }
}