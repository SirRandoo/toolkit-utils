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

using System;
using System.Diagnostics.CodeAnalysis;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Defs;
using SirRandoo.ToolkitUtils.Models;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Patches;

internal static partial class PurchaseHandlerPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("ResolvePurchaseVariables")]
    private static bool ResolvePurchaseVariablesPrefix(Viewer viewer, ITwitchMessage twitchMessage, StoreIncidentVariables incident, string formattedMessage)
    {
        if (!Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, incident.cost) || IsOnCooldown(incident, viewer))
        {
            return false;
        }

        if (!TryMakeIncident(incident, viewer, formattedMessage, out IncidentHelperVariables? inc))
        {
            TkUtils.Logger.Warn($"""The incident "{incident.defName}" does not define an incident helper""");

            return false;
        }

        Purchase_Handler.viewerNamesDoingVariableCommands.Add(viewer.username.ToLowerInvariant());

        var wasPossible = false;

        try
        {
            wasPossible = inc.IsPossible(formattedMessage, viewer);
        }
        catch (Exception e)
        {
            TkUtils.Logger.Error("Incident errored while checking if it's possible", e);
        }

        if (!wasPossible)
        {
            Purchase_Handler.viewerNamesDoingVariableCommands.Remove(viewer.username.ToLowerInvariant());

            return false;
        }

        Store_Logger.LogPurchase(viewer.username, twitchMessage.Message);
        UsageService.RecordUsage(Data.Events.Find(e => string.Equals(e.DefName, incident.defName)), viewer.username);
        Current.Game.GetComponent<Coordinator>()?.QueueIncident(new IncidentProxy { VariablesIncident = inc });
        Current.Game.GetComponent<Store_Component>()?.LogIncident(incident);

        return false;
    }

    private static bool IsOnCooldown(StoreIncidentVariables incident, Viewer viewer)
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

    private static bool TryMakeIncident(StoreIncidentVariables incident, Viewer viewer, string message, [NotNullWhen(true)] out IncidentHelperVariables? incidentHelper)
    {
        incidentHelper = StoreIncidentMaker.MakeIncidentVariables(incident);

        if (incidentHelper is null)
        {
            return false;
        }

        incidentHelper.Viewer = viewer;
        incidentHelper.message = message;

        return true;
    }
}
