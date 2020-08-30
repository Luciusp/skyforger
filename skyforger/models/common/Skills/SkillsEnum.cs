using System.ComponentModel.DataAnnotations;

namespace skyforger.models.common.Skills
{
    public enum SkillsEnum
    {
        [Skill(true, AbilityScoreEnum.Dexterity)]
        Acrobatics,

        [Skill(true, AbilityScoreEnum.Intelligence)]
        Appraise,

        [Skill(true, AbilityScoreEnum.Charisma)]
        Bluff,

        [Skill(true, AbilityScoreEnum.Strength)]
        Climb,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        Craft,

        [Skill(true, AbilityScoreEnum.Charisma)]
        Diplomacy,

        [Skill(false, AbilityScoreEnum.Dexterity)]
        [Display(Name = "Disable Device")]
        DisableDevice,

        [Skill(true, AbilityScoreEnum.Charisma)]
        Disguise,

        [Skill(true, AbilityScoreEnum.Dexterity)]
        [Display(Name = "Escape Artist")]
        EscapeArtist,

        [Skill(true, AbilityScoreEnum.Charisma)]
        [Display(Name = "Explain Everything")]
        ExplainEverything,

        [Skill(true, AbilityScoreEnum.Dexterity)]
        Fly,

        [Skill(false, AbilityScoreEnum.Charisma)] 
        [Display(Name = "Handle Animal")]
        HandleAnimal,

        [Skill(true, AbilityScoreEnum.Wisdom)]
        Heal,

        [Skill(true, AbilityScoreEnum.Charisma)]
        Intimidate,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Arcana)")]
        KnowledgeArcana,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Engineering)")]
        KnowledgeEngineering,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Geography)")]
        KnowledgeGeography,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (History)")]
        KnowledgeHistory,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Local)")]
        KnowledgeLocal,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Nature)")]
        KnowledgeNature,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Nobility)")]
        KnowledgeNobility,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Planes)")]
        KnowledgePlanes,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Religion)")]
        KnowledgeReligion,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        Linguistics,

        [Skill(true, AbilityScoreEnum.Wisdom)]
        Perception,

        [Skill(true, AbilityScoreEnum.Charisma)]
        Perform,

        [Skill(false, AbilityScoreEnum.Wisdom)]
        Profession,

        [Skill(true, AbilityScoreEnum.Dexterity)]
        Ride,

        [Skill(true, AbilityScoreEnum.Wisdom)]
        [Display(Name = "Sense Motive")]
        SenseMotive,

        [Skill(false, AbilityScoreEnum.Dexterity)]
        [Display(Name = "Sleight of Hand")]
        SleightOfHand,

        [Skill(false, AbilityScoreEnum.Intelligence)]
        Spellcraft,

        [Skill(true, AbilityScoreEnum.Dexterity)]
        Stealth,

        [Skill(true, AbilityScoreEnum.Wisdom)]
        Survival,

        [Skill(true, AbilityScoreEnum.Strength)]
        Swim,

        [Skill(false, AbilityScoreEnum.Charisma)]
        [Display(Name = "Use Magic Device")]
        UseMagicDevice,
    }
}