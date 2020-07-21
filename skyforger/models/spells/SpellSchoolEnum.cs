using System.ComponentModel.DataAnnotations;

namespace skyforger.models.spells
{
    public enum SpellSchoolEnum
    {
        None,
        Abjuration,
        Chronomancy,
        Conjuration,
        Divination,
        Enchantment,
        Evocation,
        Illusion,
        Necromancy,
        Sangromancy,
        [Display(Name = "See Text")]
        See_Text,
        Transmutation,
        Universal
    }
}