using System.ComponentModel.DataAnnotations;

namespace skyforger.models.common
{
    public enum ManaClassEnum
    {
        Mono,
        [Display(Name = "Multi-Mana")]
        Multi_mana,
        Diverse
    }
}