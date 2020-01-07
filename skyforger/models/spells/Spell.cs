using System;
using System.Collections.Generic;
using skyforger.models.common;

namespace skyforger.models.spells
{
    public class Spell
    {
        public Spell()
        {
            Mana = new List<ManaType>();
            School = new List<SpellSchool>();
            SubSchool = new List<SpellSubSchool>();
            Components = new List<SpellComponent>();
            MaterialComponents = new List<string>();
            Focus = new List<string>();
            Descriptor = new List<SpellDescriptor>();
            Action = new List<SpellAction>();
            Range = new List<SpellRange>();
            Duration = new List<DurationType>();
            SavingThrow = new List<SavingThrow>();
            SpellResistance = new List<SpellResistantType>();
        }

        public int IdHash { get; set; }
        public string Name { get; set; }
        public List<ManaType> Mana { get; set; }
        public int SpellLevel { get; set; }
        public List<SpellSchool> School { get; set; }
        public List<SpellSubSchool> SubSchool { get; set; }
        public List<SpellComponent> Components { get; set; }
        public List<string> MaterialComponents { get; set; }
        public List<string> Focus { get; set; }
        public List<SpellDescriptor> Descriptor { get; set; }
        public List<SpellAction> Action { get; set; }
        public List<SpellRange> Range { get; set; }
        public string Target { get; set; }
        public List<DurationType> Duration { get; set; }
        public List<SavingThrow> SavingThrow { get; set; }
        public List<SpellResistantType> SpellResistance { get; set; }
        public string SpellUri { get; set; }
        public string Description { get; set; }
    }
}