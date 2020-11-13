using System.ComponentModel.DataAnnotations;

namespace skyforger.models.common.Mana
{
    public enum ManaClassEnum
    {
        Mono,
        [Display(Name = "Multi-Mana")]
        MultiMana,
        Diverse
    }
}