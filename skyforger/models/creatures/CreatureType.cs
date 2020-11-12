using System.ComponentModel.DataAnnotations;

namespace skyforger.models.creatures
{
    public enum CreatureType
    {
        Aberration,
        Animal,
        Construct,
        Dragon,
        Fey,
        Humanoid,
        [Display(Name = "Magical Beast")]
        MagicalBeast,
        [Display(Name = "Monstrous Humanoid")]
        MonstrousHumanoid,
        Ooze,
        Outsider,
        Plant,
        Undead,
        Vermin
    }
}