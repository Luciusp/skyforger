using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellSchool
    {
        public SpellSchool(SpellSchoolEnum spellschoolenum)
        {
            SpellSchoolEnum = spellschoolenum;
        }

        public int Id { get; set; }
        public SpellSchoolEnum SpellSchoolEnum { get; set; }
    }
}