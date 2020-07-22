using System.ComponentModel.DataAnnotations;

namespace skyforger.models.common
{
    public enum SkillsEnum
    {
        [SkillAttribute(true, AbilityScoreEnum.Dexterity)]
        Acrobatics,

        [SkillAttribute(true, AbilityScoreEnum.Intelligence)]
        Appraise,

        [SkillAttribute(true, AbilityScoreEnum.Charisma)]
        Bluff,

        [SkillAttribute(true, AbilityScoreEnum.Strength)]
        Climb,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        Craft,

        [SkillAttribute(true, AbilityScoreEnum.Charisma)]
        Diplomacy,

        [SkillAttribute(false, AbilityScoreEnum.Dexterity)]
        [Display(Name = "Disable Device")]
        DisableDevice,

        [SkillAttribute(true, AbilityScoreEnum.Charisma)]
        Disguise,

        [SkillAttribute(true, AbilityScoreEnum.Dexterity)]
        [Display(Name = "Escape Artist")]
        EscapeArtist,

        [SkillAttribute(true, AbilityScoreEnum.Charisma)]
        [Display(Name = "Explain Everything")]
        ExplainEverything,

        [SkillAttribute(true, AbilityScoreEnum.Dexterity)]
        Fly,

        [SkillAttribute(false, AbilityScoreEnum.Charisma)] 
        [Display(Name = "Handle Animal")]
        HandleAnimal,

        [SkillAttribute(true, AbilityScoreEnum.Wisdom)]
        Heal,

        [SkillAttribute(true, AbilityScoreEnum.Charisma)]
        Intimidate,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Arcana)")]
        KnowledgeArcana,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Engineering)")]
        KnowledgeEngineering,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Geography)")]
        KnowledgeGeography,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (History)")]
        KnowledgeHistory,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Local)")]
        KnowledgeLocal,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Nature)")]
        KnowledgeNature,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Nobility)")]
        KnowledgeNobility,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Planes)")]
        KnowledgePlanes,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        [Display(Name = "Knowledge (Religion)")]
        KnowledgeReligion,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        Linguistics,

        [SkillAttribute(true, AbilityScoreEnum.Wisdom)]
        Perception,

        [SkillAttribute(true, AbilityScoreEnum.Charisma)]
        Perform,

        [SkillAttribute(false, AbilityScoreEnum.Wisdom)]
        Profession,

        [SkillAttribute(true, AbilityScoreEnum.Dexterity)]
        Ride,

        [SkillAttribute(true, AbilityScoreEnum.Wisdom)]
        [Display(Name = "Sense Motive")]
        SenseMotive,

        [SkillAttribute(false, AbilityScoreEnum.Dexterity)]
        [Display(Name = "Sleight of Hand")]
        SleightOfHand,

        [SkillAttribute(false, AbilityScoreEnum.Intelligence)]
        Spellcraft,

        [SkillAttribute(true, AbilityScoreEnum.Dexterity)]
        Stealth,

        [SkillAttribute(true, AbilityScoreEnum.Wisdom)]
        Survival,

        [SkillAttribute(true, AbilityScoreEnum.Strength)]
        Swim,

        [SkillAttribute(false, AbilityScoreEnum.Charisma)]
        [Display(Name = "Use Magic Device")]
        UseMagicDevice,
    }
}