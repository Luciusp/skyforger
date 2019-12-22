using System;
using skyforger.models.creatures;

namespace skyforger.models
{
    public class GeneratorQuery
    {
        public string Race { get; set; }
        public string Class { get; set; }
        public ManaType ManaType { get; set; }
        public CreatureAlignment Alignment { get; set; }
        public int Level { get; set; }
        public int BasePoints { get; set; }
    }
}