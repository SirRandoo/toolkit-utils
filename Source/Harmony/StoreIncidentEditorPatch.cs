using System.Linq;
using HarmonyLib;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Store_IncidentEditor), "UpdatePriceSheet")]
    public static class StoreIncidentEditorPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            var incidents = DefDatabase<StoreIncident>.AllDefs;

            foreach (var incident in incidents.Where(
                i => i.defName == "BuyPawn"
                     || i.defName == "AddTrait"
                     || i.defName == "RemoveTrait"
                     || i.defName == "ReplaceTrait"
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
