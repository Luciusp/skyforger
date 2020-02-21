using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellSubSchool
    {
        public SpellSubSchool(SpellSubSchoolEnum spellsubschoolenum)
        {
            SpellSubSchoolEnum = spellsubschoolenum;
        }
        public int Id { get; set; }
        public SpellSubSchoolEnum SpellSubSchoolEnum { get; set; }
    }
}