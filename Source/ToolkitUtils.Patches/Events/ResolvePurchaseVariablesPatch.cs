// MIT License
// 
// Copyright (c) 2022 SirRandoo
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
