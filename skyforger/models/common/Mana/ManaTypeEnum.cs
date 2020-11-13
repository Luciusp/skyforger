using System.ComponentModel.DataAnnotations;

namespace skyforger.models.common.Mana
{
    public enum ManaTypeEnum
    {
        White,
        Black,
        Blue,
        Red,
        Green,
        [Display(Name = "See Text")]
        SeeText
    }
}