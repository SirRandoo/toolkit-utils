// ToolkitUtils
// Copyright (C) 2021  SirRandoo
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.

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
        public static bool Prefix(Viewer viewer, [NotNull] ITwitchMessage twitchMessage, bool separateChannel = false)
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

        private static bool TryFindVariableIncident(
            string query,
            [CanBeNull] out StoreIncidentVariables incidentVariables
        )
        {
            incidentVariables = Purchase_Handler.allStoreIncidentsVariables
               .Where(i => i.cost > 0 || i.defName.Equals("Item"))
               .FirstOrDefault(i => query.EqualsIgnoreCase(i.abbreviation));

            return incidentVariables != null;
        }

        private static bool TryFindSimpleIncident(string query, [CanBeNull] out StoreIncidentSimple incidentSimple)
        {
            incidentSimple = Purchase_Handler.allStoreIncidentsSimple.Where(i => i.cost > 0 || i.defName.Equals("Item"))
               .FirstOrDefault(i => query.EqualsIgnoreCase(i.abbreviation));

            return incidentSimple != null;
        }

        private static bool TryFindItem(string query, [CanBeNull] out ThingItem item)
        {
            item = Data.Items.Where(i => i.Cost > 0).FirstOrDefault(i => query.EqualsIgnoreCase(i.Name));
            return item != null;
        }
    }
}
