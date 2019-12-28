using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace skyforger.models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum ManaType
    {
        White,
        Black,
        Blue,
        Red,
        Green,
        See_Text
    }
}