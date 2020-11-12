using System.ComponentModel.DataAnnotations;

namespace skyforger.models.creatures
{
    public enum CreatureSize
    {
        Fine,
        Diminutive,
        Tiny,
        Small,
        Medium,
        [Display(Name = "Large (Tall)")]
        LargeTall,
        [Display(Name = "Large (Long)")]
        LargeLong,
        [Display(Name = "Huge (Tall)")]
        HugeTall,
        [Display(Name = "Huge (Long)")]
        HugeLong,
        [Display(Name = "Gargantuan (Tall)")]
        GargantuanTall,
        [Display(Name = "Gargantuan (Long)")]
        GargantuanLong,
        [Display(Name = "Colossal (Tall)")]
        ColossalTall,
        [Display(Name = "Colossal (Long)")]
        ColossalLong
    }
}