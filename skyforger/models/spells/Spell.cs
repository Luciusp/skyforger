using System.Collections.Generic;
using Newtonsoft.Json;
using skyforger.models.common.Mana;

namespace skyforger.models.spells
{
    public class Spell
    {
        public Spell()
        {
            Mana = new List<ManaType>();
            ManaClass = new List<ManaClass>();
            School = new List<SpellSchool>();
            SubSchool = new List<SpellSubSchool>();
            Components = new List<SpellComponent>();
            MaterialComponents = new List<MaterialComponent>();
            Focus = new List<Focus>();
            Descriptor = new List<SpellDescriptor>();
            Action = new List<SpellAction>();
            //Range = new List<SpellRange>();
            //Duration = new List<DurationType>();
            //SavingThrow = new List<SavingThrow>();
            //SpellResistance = new List<SpellResistantType>();
        }
        
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ManaType> Mana { get; set; }
        public List<ManaClass> ManaClass { get; set; }
        public string ManaDescription { get; set; }
        public int SpellLevel { get; set; }
        public string SchoolRaw { get; set; }
        public List<SpellSchool> School { get; set; }
        public List<SpellSubSchool> SubSchool { get; set; }
        public List<SpellComponent> Components { get; set; }
        public List<MaterialComponent> MaterialComponents { get; set; }
        public List<Focus> Focus { get; set; }
        public List<SpellDescriptor> Descriptor { get; set; }
        public List<SpellAction> Action { get; set; }
        //public List<SpellRange> Range { get; set; }
        public string Range { get; set; }
        public string Target { get; set; }
        //public List<DurationType> Duration { get; set; }
        public string Duration { get; set; }
        public string Effect { get; set; }
        public string SavingThrow { get; set; }
        //public List<SavingThrow> SavingThrow { get; set; }
        //public List<SpellResistantType> SpellResistance { get; set; }
        public string SpellResistance { get; set; }
        public string SpellUri { get; set; }
        public string Description { get; set; }
        
        [JsonIgnore]
        public bool Valid { get; set; }
    }
}