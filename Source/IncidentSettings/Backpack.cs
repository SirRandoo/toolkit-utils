using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.IncidentSettings.Windows;
using TwitchToolkit.Incidents;
using Verse;

namespace SirRandoo.ToolkitUtils.IncidentSettings
{
    [UsedImplicitly(ImplicitUseKindFlags.InstantiatedWithFixedConstructorSignature)]
    public class Backpack : IncidentHelperVariablesSettings
    {
        public static bool AutoEquip = true;

        public override void ExposeData()
        {
            Scribe_Values.Look(ref AutoEquip, "backpackAutoEquip", true);
        }

        public override void EditSettings()
        {
            Find.WindowStack.Add(new BackpackDialog());
        }
    }
}
