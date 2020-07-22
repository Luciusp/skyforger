using System;

namespace skyforger.models.common
{
    [AttributeUsage(AttributeTargets.Field)]
    public class SkillAttribute : Attribute
    {
        public static bool Untrained;
        public static AbilityScoreEnum DependentStat;

        public SkillAttribute(bool untrained, AbilityScoreEnum dependentstat)
        {
            Untrained = untrained;
            DependentStat = dependentstat;
        }
    }
}