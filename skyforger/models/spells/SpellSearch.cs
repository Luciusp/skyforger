using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using skyforger.models.common;

namespace skyforger.models.spells
{
    public class SpellSearch
    {
        [Display(Name = "Mana Color")]
        public ManaTypeEnum? ManaColor { get; set; }
        
        [Display(Name = "Mana Class")] 
        public ManaClassEnum? ManaClass { get; set; }

        [Display(Name = "Spell Level Range")]
        public int? SpellLevelLowerBound { get; set; }
        public int? SpellLevelUpperBound { get; set; }
        
        [Display(Name = "Spell School")]
        public SpellSchoolEnum? SpellSchool { get; set; }

        [Display(Name = "Spell Subschool")]
        public SpellSubSchoolEnum? SpellSubSchool { get; set; }
        
        [Display(Name = "Spell Descriptor")]
        public SpellDescriptorEnum? SpellDescriptor { get; set; }
        
        [Display(Name = "Title Contains The Words")]
        public string TitleContainsWords { get; set; }
        
        [Display(Name = "Description Contains the Words")]
        public string DescriptionContainsWords { get; set; }
        
        [Display(Name = "Fuzzy Match Description")]
        public bool FuzzyMatchDescription { get; set; }
        
        [Display(Name = "Randomize Selection (20 spells max)")]
        public bool IsRandom { get; set; }
        
    }
}