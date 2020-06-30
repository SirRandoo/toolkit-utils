using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_IncidentEditor), "UpdatePriceSheet")]
    [UsedImplicitly]
    public static class StoreIncidentEditorPatch
    {
        [HarmonyPrefix]
        [UsedImplicitly]
        public static void Prefix()
        {
            IEnumerable<StoreIncident> incidents = DefDatabase<StoreIncident>.AllDefs;

            foreach (StoreIncident incident in incidents.Where(
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
