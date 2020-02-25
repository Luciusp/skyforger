using System.Collections.Generic;

namespace skyforger.models.spells
{
    public class SpellQueryResult
    {
        public SpellQueryResult(Dictionary<string, int> cc, List<Spell> spells)
        {
            ColorCounts = cc;
            Spells = spells;
        }

        public Dictionary<string, int> ColorCounts { get; set; }
        public List<Spell> Spells { get; set; }
    }
}