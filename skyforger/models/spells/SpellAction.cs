using skyforger.models.common;

namespace skyforger.models.spells
{
    public class SpellAction
    {
        public int Id { get; set; }
        public ActionType Type { get; set; }
        public int TimeFactor { get; set; }
    }
}