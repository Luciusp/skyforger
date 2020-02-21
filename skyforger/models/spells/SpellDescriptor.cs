using skyforger.models.spells;

namespace skyforger.models
{
    public class SpellDescriptor
    {
        public SpellDescriptor(SpellDescriptorEnum spelldescriptorenum)
        {
            SpellDescriptorEnum = spelldescriptorenum;
        }

        public int Id { get; set; }
        public SpellDescriptorEnum SpellDescriptorEnum { get; set; }
    }
}