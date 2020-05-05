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
            var incidents = DefDatabase<StoreIncident>.AllDefsListForReading;

            foreach (var incident in incidents)
            {
                switch (incident.defName)
                {
                    case "BuyPawn":
                    case "AddTrait":
                    case "RemoveTrait":
                        incident.cost = 1;
                        return;
                }
            }
        }
    }
}
