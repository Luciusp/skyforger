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
        LanguageDependent,
        Lawful,
        Light,
        Maneuver,
        [Display(Name = "Mind Affecting")]
        MindAffecting,
        Pain,
        Poison,
        Profane,
        Runic,
        Sacred,
        Scrying,
        [Display(Name = "See Text")]
        SeeText,
        Shadow,
        Sonic,
        Spirit,
        Symbol,
        Water,
        Wild
    }
}