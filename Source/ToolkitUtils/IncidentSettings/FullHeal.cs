using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.IncidentSettings.Windows;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class FullHeal : IncidentHelperVariablesSettings
    {
        public static bool FairFights;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref FairFights, "fullHealFairFights");
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new HealMeDialog());
        }
    }
}
