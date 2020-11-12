using System.Collections.Generic;
using skyforger.models.common;
using skyforger.models.common.Skills;

namespace skyforger.models.classes
{
    public static class Battlecaster
    {
        public static string Name = "Battlecaster";
        public static int[] BasicAttackBonus = {0,1,2,3,3,4,5,6,6,7,8,9,9,10,11,12,12,13,14,15};
        public static int[] FortitudeBonus = {2,3,3,4,4,5,5,6,6,7,7,8,8,9,9,10,10,11,11,12};
        public static int[] ReflexBonus = {0,0,1,1,1,2,2,2,3,3,3,4,4,4,6,6,6};
        public static int[] WillBonus = {2,3,3,4,4,5,5,6,6,7,7,8,8,9,9,10,10,11,11,12};
        public static int[] ManaPoints = {8,16,26,36,48,60,74,88,104,120,138,156,176,196,218,240,264,288,314,340};
        public static int HitDie = 8;
        public static List<SkillsEnum> ClassSkills = new List<SkillsEnum>()
        {
            SkillsEnum.Climb, 
            SkillsEnum.Craft, 
            SkillsEnum.KnowledgeArcana, 
            SkillsEnum.KnowledgeEngineering,
            SkillsEnum.KnowledgeGeography,
            SkillsEnum.KnowledgeHistory,
            SkillsEnum.KnowledgeLocal,
            SkillsEnum.KnowledgeNature,
            SkillsEnum.KnowledgeNobility,
            SkillsEnum.KnowledgePlanes,
            SkillsEnum.KnowledgeReligion
        };
        public static int SkillRanksPerLevel = 4; //plus int modifier. Done at char gen
        public static List<Proficiency> Proficiencies = new List<Proficiency>()
        {
            Proficiency.SimpleWeapon,
            Proficiency.MartialWeapon,
            Proficiency.LightArmor,
            Proficiency.MediumArmor,
            Proficiency.Shield
        };

        public static int[] BonusFeatsGainedAtLevel = {4,8,12,16,20};

        public static void AddClass()
        {
            
        }
    }
}