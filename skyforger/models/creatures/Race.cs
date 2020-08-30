using System.ComponentModel.DataAnnotations;

namespace skyforger.models.creatures
{
    public enum Race
    {
        Human,
        Elf,
        Goblin,
        Merfolk,
        Barbarian,
        Dwarf,
        Minotaur,
        Kenku,
        Nacatl,
        Nishoba,
        Loxodon,
        [Display(Name = "Half-elf")]
        HalfElf,
        Tengu
    }
}