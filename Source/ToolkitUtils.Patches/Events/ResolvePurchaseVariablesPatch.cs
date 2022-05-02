// ToolkitUtils
// Copyright (C) 2022  SirRandoo
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

using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    internal static partial class PurchaseHandlerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ResolvePurchaseVariables")]
        private static bool ResolvePurchaseVariablesPrefix(
            [NotNull] Viewer viewer,
            ITwitchMessage twitchMessage,
            [NotNull] StoreIncidentVariables incident,
            string formattedMessage
        )
        {
            if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, incident.cost) || IsOnCooldown(incident, viewer))
            {
                return false;
            }

            if (!TryMakeIncident(incident, viewer, formattedMessage, out IncidentHelperVariables inc))
            {
                TkUtils.Logger.Warn(@$"The incident ""{incident.defName}"" does not define an incident helper");

                return false;
            }

            Purchase_Handler.viewerNamesDoingVariableCommands.Add(viewer.username.ToLowerInvariant());

            if (!inc.IsPossible(formattedMessage, viewer))
            {
                Purchase_Handler.viewerNamesDoingVariableCommands.Remove(viewer.username.ToLowerInvariant());

                return false;
            }

            Store_Logger.LogPurchase(viewer.username, twitchMessage.Message);

            Current.Game.GetComponent<Coordinator>()?.QueueIncident(new IncidentProxy { VariablesIncident = inc });
            Current.Game.GetComponent<Store_Component>()?.LogIncident(incident);

            return false;
        }
        private static bool IsOnCooldown(StoreIncidentVariables incident, [NotNull] Viewer viewer)
        {
            if (incident == StoreIncidentDefOf.Item)
            {
                return Purchase_Handler.CheckIfCarePackageIsOnCooldown(viewer.username);
            }

            if (incident != IncidentDefOf.Sanctuary && Purchase_Handler.CheckIfKarmaTypeIsMaxed(incident, viewer.username))
            {
                return true;
            }

            return Purchase_Handler.CheckIfIncidentIsOnCooldown(incident, viewer.username);
        }

        [ContractAnnotation("=> true,incidentHelper:notnull; => false,incidentHelper:null")]
        private static bool TryMakeIncident(StoreIncidentVariables incident, Viewer viewer, string message, out IncidentHelperVariables incidentHelper)
        {
            incidentHelper = StoreIncidentMaker.MakeIncidentVariables(incident);

            if (incidentHelper == null)
            {
                return false;
            }

            incidentHelper.Viewer = viewer;
            incidentHelper.message = message;

            return true;
        }
    }
}
