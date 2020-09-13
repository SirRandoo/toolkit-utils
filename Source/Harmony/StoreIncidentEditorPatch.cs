using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_IncidentEditor), "UpdatePriceSheet")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class StoreIncidentEditorPatch
    {
        public static void Prefix()
        {
            foreach (StoreIncident incident in DefDatabase<StoreIncident>.AllDefs.Where(
                i => (i.GetModExtension<EventExtension>()?.EventType ?? EventTypes.None) != EventTypes.None
            ))
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                incident.cost = 1;
            }
        }
    }
}
