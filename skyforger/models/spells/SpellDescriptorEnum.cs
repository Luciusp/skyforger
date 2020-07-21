using System.ComponentModel.DataAnnotations;

namespace skyforger.models.spells
{
    public enum SpellDescriptorEnum
    {
        None,
        Acid,
        Air,
        Chaotic,
        Cold,
        Curse,
        Darkness,
        Death,
        Disease,
        Draconic,
        Earth,
        Electricity,
        Emotion,
        Evil,
        Fairy,
        Fear,
        Fire,
        Force,
        Good,
        Glyph,
        [Display(Name = "Language Dependent")]
        Language_Dependent,
        Lawful,
        Light,
        Maneuver,
        [Display(Name = "Mind Affecting")]
        Mind_Affecting,
        Pain,
        Poison,
        Profane,
        Runic,
        Sacred,
        Scrying,
        [Display(Name = "See Text")]
        See_Text,
        Shadow,
        Sonic,
        Spirit,
        Symbol,
        Water,
        Wild
    }
}