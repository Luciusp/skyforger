using System.ComponentModel.DataAnnotations;

namespace skyforger.models.creatures
{
    public enum CreatureAlignment
    {
        [Display(Name = "Lawful Good")]
        LawfulGood,
        [Display(Name = "Lawful Neutral")]
        LawfulNeutral,
        [Display(Name = "Lawful Evil")]
        LawfulEvil,
        [Display(Name = "Chaotic Good")]
        ChaoticGood,
        [Display(Name = "Chaotic Neutral")]
        ChaoticNeutral,
        [Display(Name = "Chaotic Evil")]
        ChaoticEvil,
        [Display(Name = "Neutral Good")]
        NeutralGood,
        [Display(Name = "True Neutral")]
        TrueNeutral,
        [Display(Name = "Neutral Evil")]
        NeutralEvil
    }
}