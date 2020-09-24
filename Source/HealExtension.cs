using JetBrains.Annotations;
using Verse;

namespace SirRandoo.ToolkitUtils
{
    [UsedImplicitly(ImplicitUseKindFlags.Assign, ImplicitUseTargetFlags.WithMembers)]
    public class HealExtension : DefModExtension
    {
        [Description("Whether or not Utils' heal events should heal this hediff.")]
        [DefaultValue(true)]
        public bool ShouldHeal = true;
    }
}
