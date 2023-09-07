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

using System.Diagnostics.CodeAnalysis;
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

namespace SirRandoo.ToolkitUtils.Patches;

internal static partial class PurchaseHandlerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("ResolvePurchaseSimple")]
    private static bool ResolvePurchaseSimplePrefix(
        Viewer viewer,
        ITwitchMessage twitchMessage,
        StoreIncidentSimple incident,
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

        if (Purchase_Handler.CheckIfIncidentIsOnCooldown(incident, viewer.username))
        {
            return false;
        }

        if (!TryMakeIncident(incident, viewer, formattedMessage, out IncidentHelper? inc))
        {
            TkUtils.Logger.Warn($"""The incident "{incident.defName}" does not define an incident helper""");

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
        UsageService.RecordUsage(Data.Events.Find(e => string.Equals(e.DefName, incident.defName)), viewer.username);

        if (!ToolkitSettings.PurchaseConfirmations)
        {
            return false;
        }

        TwitchWrapper.SendChatMessage(
            Helper.ReplacePlaceholder("TwitchToolkitEventPurchaseConfirm".Localize(), viewer: viewer.username, first: incident.label.CapitalizeFirst())
        );

        return false;
    }

    private static bool TryMakeIncident(StoreIncidentSimple incident, Viewer viewer, string message, [NotNullWhen(true)] out IncidentHelper? incidentHelper)
    {
        incidentHelper = StoreIncidentMaker.MakeIncident(incident);

        if (incidentHelper is null)
        {
            return false;
        }

        incidentHelper.Viewer = viewer;
        incidentHelper.message = message;

        return true;
    }
}
