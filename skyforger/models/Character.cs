using skyforger.models.creatures;
using skyforger.models.spells;

namespace skyforger.models
{
    public class Character
    {
        public CharacterRace Race { get; set; }
        public CreatureType Type { get; set; }
        public CreatureSubType SubType { get; set; }
        public CreatureSize Size { get; set; }
        public CreatureAlignment Alignment { get; set; }
        public CharacterClass Class { get; set; }
        public Statblock Statblock { get; set; }
        public Spell[] CharacterSpells { get; set; }
    }
}