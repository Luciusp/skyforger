using System.ComponentModel.DataAnnotations;

namespace skyforger.models.spells
{
    public enum SpellSubSchoolEnum
    {
        //Catch-all
        None,
        [Display(Name = "See Text")]
        See_Text,
        
        //Conjuration
        Calling,
        Creation,
        Healing,
        Summoning,
        Teleportation,
        
        //Divination
        Scrying,
        
        //Enchantment
        Charm,
        Compulsion,
        
        //Evocation
        
        //Illusion
        Figment,
        Glamer,
        Pattern,
        Phantasm,
        Shadow,
        
        //Necromancy
        
        //Sangromancy
        
        //Transmutation
        Polymorph
    }
}