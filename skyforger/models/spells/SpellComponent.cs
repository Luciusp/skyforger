using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellComponent
    {
        public SpellComponent(SpellComponentEnum spellcomponentenum)
        {
            SpellComponentEnum = spellcomponentenum;
        }

        public int Id { get; set; }
        public SpellComponentEnum SpellComponentEnum { get; set; }
    }
}