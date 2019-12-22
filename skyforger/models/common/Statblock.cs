using System.Net.Sockets;

namespace skyforger.models
{
    public class Statblock
    {
        public Statblock(CharacterClass characterclass, CharacterRace characterrace, int basepoints, int level)
        {
            
        }
        
        public int Strength { get; set; }
        public int StrengthMod { get; set; }
        
        public int Dexterity { get; set; }
        public int DexterityMod { get; set; }
        
        public int Constitution { get; set; }
        public int ConstitutionMod { get; set; }
        
        public int Intelligence { get; set; }
        public int IntelligenceMod { get; set; }
        
        public int Wisdom { get; set; }
        public int WisdomMod { get; set; }
        
        public int Charisma { get; set; }
        public int CharismaMod { get; set; }

    }
    
}