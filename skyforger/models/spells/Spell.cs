using System;

namespace skyforger.models.spells
{
    public class Spell
    {
        public string Name { get; set; }
        public ManaType[] Mana { get; set; }
        public int SpellLevel { get; set; }
        public SpellSchool School { get; set; }
        public SpellSubSchool SubSchool { get; set; }
        public SpellComponent[] Components { get; set; }
        public SpellDescriptor Descriptor { get; set; }
        public Action CastingTime { get; set; }
        public string Range { get; set; }
        public string Target { get; set; }
        public string Duration { get; set; }
        public string SavingThrow { get; set; }
        public string SpellResistance { get; set; }
        public string SpellUri { get; set; }
        public string Description { get; set; }
    }
}