using skyforger.models.common;
using skyforger.models.creatures;
using skyforger.models.spells;

namespace skyforger.models
{
    public class Character
    {
        public Character()
        {
            
        }
        
        public CharacterRace Race { get; set; }
        public CreatureType Type { get; set; }
        public CreatureSubType SubType { get; set; }
        public CreatureSize Size { get; set; }
        public CreatureAlignment Alignment { get; set; }
        public ManaType ManaAffinity { get; set; }
        public int Level { get; set; }
        public CharacterClass Class { get; set; }
        public Statblock Statblock { get; set; }
        public Spell[] CharacterSpells { get; set; }
        public int Basic_Attack_Bonus { get; set; }
        public int MaxHitPoints { get; set; }
        public int CurrentHitPoints { get; set; }
        public int Combat_Maneuver_Bonus { get; set; }
        public int Combat_Maneuver_Defense { get; set; }
        public int Initiative { get; set; }
        public int Speed { get; set; }
        public int ArmorClass { get; set; }
        public int FlatFooted { get; set; }
        public int Touch { get; set; }
        public CreatureSaves Saves { get; set; }
        public int MaxMana { get; set; }
        public int CurrentMana { get; set; }
        public Languages[] Languages { get; set; }
        public int FeatCount { get; set; }
        public Feat[] Feats { get; set; }
        public int TraitCount { get; set; }
        public Trait[] Traits { get; set; }
        public CreatureEquipmentSlots EquipmentSlots { get; set; }
        public int Height { get; set; }
        public int LightLoad { get; set; }
        public int MediumLoad { get; set; }
        public int HeavyLoad { get; set; }
        
        private void CalculateAllTheThings()
        {
            //potentially not randoms:
            //pick race
            //pick class
            //pick mana
            //pick alignment
            //pick level
            //pick basepoints
            
            //BAB
            //MHP
            //CMB
            //CMD
            //Init
            //Speed
            //AC
            //FF
            //Touch
            //Saves
            //MaxMana
            //CurrentMana
            //Languages
            //FeatCount
            //TraitCount
            //Height
            //LightLoad
            //MediumLoad
            //HeavyLoad
        }
    }
}