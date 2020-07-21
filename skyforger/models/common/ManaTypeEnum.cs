using System.ComponentModel.DataAnnotations;

namespace skyforger.models.common
{
    public enum ManaTypeEnum
    {
        White,
        Black,
        Blue,
        Red,
        Green,
        [Display(Name = "See Text")]
        See_Text
    }
}