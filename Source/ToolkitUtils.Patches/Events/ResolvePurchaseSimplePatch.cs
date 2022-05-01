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
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using ToolkitCore;
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
        [HarmonyPatch("ResolvePurchaseSimple")]
        private static bool ResolvePurchaseSimplePrefix(
            [NotNull] Viewer viewer,
            ITwitchMessage twitchMessage,
            [NotNull] StoreIncidentSimple incident,
            string formattedMessage
        )
        {
            if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, incident.cost))
            {
                return false;
            }

            if (Purchase_Handler.CheckIfKarmaTypeIsMaxed(incident, viewer.username))
            {
                return false;
            }

            if (!TryMakeIncident(incident, viewer, formattedMessage, out IncidentHelper inc))
            {
                TkUtils.Logger.Warn(@$"The incident ""{incident.defName}"" does not define an incident helper");

                return false;
            }

            if (!inc.IsPossible())
            {
                MessageHelper.ReplyToUser(viewer.username, "TwitchToolkitEventNotPossible".Localize());

                return false;
            }

            viewer.Charge(incident);

            Current.Game.GetComponent<Coordinator>()?.QueueIncident(new IncidentProxy { SimpleIncident = inc });
            Current.Game.GetComponent<Store_Component>()?.LogIncident(incident);

            Store_Logger.LogPurchase(viewer.username, twitchMessage.Message);

            if (!ToolkitSettings.PurchaseConfirmations)
            {
                return false;
            }

            TwitchWrapper.SendChatMessage(
                Helper.ReplacePlaceholder("TwitchToolkitEventPurchaseConfirm".Localize(), viewer: viewer.username, first: incident.label.CapitalizeFirst())
            );

            return false;
        }

        [ContractAnnotation("=> true,incidentHelper:notnull; => false,incidentHelper:null")]
        private static bool TryMakeIncident(StoreIncidentSimple incident, Viewer viewer, string message, out IncidentHelper incidentHelper)
        {
            incidentHelper = StoreIncidentMaker.MakeIncident(incident);

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
