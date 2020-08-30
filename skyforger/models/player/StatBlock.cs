using System;

namespace skyforger.models.player
{
    public class StatBlock
    {
        public int StrengthBase { get; set; } = 10;
        public int StrengthMagic { get; set; } = 0;
        public int StrengthScore => StrengthBase + StrengthMagic;
        public int StrengthMod => (int) Math.Floor((double) (StrengthScore - 10) / 2);

        public int DexterityBase { get; set; } = 10;
        public int DexterityMagic { get; set; } = 0;
        public int DexterityScore => DexterityBase + DexterityMagic;
        public int DexterityMod => (int) Math.Floor((double) (DexterityScore - 10) / 2);

        public int ConstitutionBase { get; set; } = 10;
        public int ConstitutionMagic { get; set; } = 0;
        public int ConstitutionScore => ConstitutionBase + ConstitutionMagic;
        public int ConstitutionMod => (int) Math.Floor((double) (ConstitutionScore - 10) / 2);

        public int IntelligenceBase { get; set; } = 10;
        public int IntelligenceMagic { get; set; } = 0;
        public int IntelligenceScore => IntelligenceBase + IntelligenceMagic;
        public int IntelligenceMod => (int) Math.Floor((double) (IntelligenceScore - 10) / 2);

        public int WisdomBase { get; set; } = 10;
        public int WisdomMagic { get; set; } = 0;
        public int WisdomScore => WisdomBase + WisdomMagic;
        public int WisdomMod => (int) Math.Floor((double) (WisdomScore - 10) / 2);

        public int CharismaBase { get; set; } = 10;
        public int CharismaMagic { get; set; } = 0;
        public int CharismaScore => CharismaBase + CharismaMagic;
        public int CharismaMod => (int) Math.Floor((double) (CharismaScore - 10) / 2);
    }
}