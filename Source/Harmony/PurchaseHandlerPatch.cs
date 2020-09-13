using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Purchase_Handler), "ResolvePurchase")]
    [UsedImplicitly(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.WithMembers)]
    public static class PurchaseHandlerPatch
    {
        public static bool Prefix(Viewer viewer, ITwitchMessage twitchMessage, bool separateChannel = false)
        {
            List<string> segments = CommandFilter.Parse(twitchMessage.Message).ToList();
            string query = segments.Skip(1).FirstOrFallback("");

            if (segments.Count < 2)
            {
                return false;
            }

            if (TryFindSimpleIncident(query, out StoreIncidentSimple incidentSimple))
            {
                Purchase_Handler.ResolvePurchaseSimple(viewer, twitchMessage, incidentSimple, twitchMessage.Message);
                return false;
            }

            if (TryFindVariableIncident(query, out StoreIncidentVariables incidentVariables))
            {
                Purchase_Handler.ResolvePurchaseVariables(
                    viewer,
                    twitchMessage,
                    incidentVariables,
                    twitchMessage.Message
                );
                return false;
            }

            Helper.Log($"abr: {query} ");
            if (!TryFindItem(query, out ThingItem _))
            {
                return false;
            }

            segments.Insert(1, "item");
            if (segments.Count < 4)
            {
                segments.Add("1");
            }

            if (!int.TryParse(segments[3], out int _))
            {
                segments.Insert(3, "1");
            }

            Purchase_Handler.ResolvePurchaseVariables(
                viewer,
                twitchMessage,
                StoreIncidentDefOf.Item,
                string.Join(" ", segments.ToArray())
            );

            return false;
        }

        private static bool TryFindVariableIncident(string query, out StoreIncidentVariables incidentVariables)
        {
            incidentVariables =
                Purchase_Handler.allStoreIncidentsVariables.FirstOrDefault(i => query.EqualsIgnoreCase(i.abbreviation));

            return incidentVariables != null;
        }

        private static bool TryFindSimpleIncident(string query, out StoreIncidentSimple incidentSimple)
        {
            incidentSimple =
                Purchase_Handler.allStoreIncidentsSimple.FirstOrDefault(i => query.EqualsIgnoreCase(i.abbreviation));

            return incidentSimple != null;
        }

        private static bool TryFindItem(string query, out ThingItem item)
        {
            item = Data.Items.FirstOrDefault(i => query.EqualsIgnoreCase(i.Name));
            return item != null;
        }
    }
}
