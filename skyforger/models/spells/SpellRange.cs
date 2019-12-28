using skyforger.models.common;

namespace skyforger.models.spells
{
    public class SpellRange
    {
        public RangeType Range { get; set; }
        public string OtherRange { get; set; }
        public int increment { get; set; }
    }
}