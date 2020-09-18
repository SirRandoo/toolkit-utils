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
            foreach (StoreIncident incident in DefDatabase<StoreIncident>.AllDefs)
            {
                if (incident.cost <= 1)
                {
                    continue;
                }

                EventTypes type = incident.GetModExtension<EventExtension>()?.EventType ?? EventTypes.None;

                if (type == EventTypes.None || type == EventTypes.Variable)
                {
                    continue;
                }

                incident.cost = 1;
            }
        }
    }
}
