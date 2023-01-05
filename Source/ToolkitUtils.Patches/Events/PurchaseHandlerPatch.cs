﻿// ToolkitUtils
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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Patches
{
    /// <summary>
    ///     A Harmony patch for adjusting how Twitch Toolkit's purchase
    ///     handler processes events to support the various changes Utils'
    ///     requires.
    /// </summary>
    /// <remarks>
    ///     Originally, this patch was created to support the
    ///     <see cref="Coordinator"/>, but more recently it was extended to
    ///     support Utils' cooldown system.
    /// </remarks>
    [HarmonyPatch(typeof(Purchase_Handler))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    internal static partial class PurchaseHandlerPatch
    {
        [CanBeNull]
        private static Exception Cleanup(MethodBase original, [CanBeNull] Exception exception)
        {
            if (exception == null)
            {
                return null;
            }

            TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

            return null;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Purchase_Handler.ResolvePurchase))]
        private static bool ResolvePurchasePrefix([NotNull] Viewer viewer, [NotNull] ITwitchMessage twitchMessage)
        {
            if (Purchase_Handler.CheckIfViewerIsInVariableCommandList(viewer.username))
            {
                return false;
            }

            List<string> segments = CommandFilter.Parse(twitchMessage.Message).ToList();
            var worker = ArgWorker.CreateInstance(segments);

            if (!worker.HasNext())
            {
                return false;
            }

            string query = segments.Skip(1).FirstOrFallback("");

            if (TryProcessIncident(viewer, twitchMessage, query))
            {
                return false;
            }

            Helper.Log($"abr: {query} ");
            
            segments.Insert(1, "item");

            if (segments.Count < 4)
            {
                segments.Add("1");
            }

            if (!int.TryParse(segments[3], out int _))
            {
                segments.Insert(3, string.Equals(segments[3], "*") ? "*" : "1");
            }

            try
            {
                Purchase_Handler.ResolvePurchaseVariables(viewer, twitchMessage, StoreIncidentDefOf.Item, string.Join(" ", segments.ToArray()));
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not resolve purchase", e);
            }

            return false;
        }

        private static bool TryProcessIncident(Viewer viewer, ITwitchMessage twitchMessage, string query) =>
            TryProcessSimpleIncident(viewer, twitchMessage, query) || TryProcessVariablesIncident(viewer, twitchMessage, query);

        private static bool TryProcessVariablesIncident(Viewer viewer, ITwitchMessage twitchMessage, string query)
        {
            if (!TryFindVariableIncident(query, out StoreIncidentVariables incidentVariables))
            {
                return false;
            }

            try
            {
                Purchase_Handler.ResolvePurchaseVariables(viewer, twitchMessage, incidentVariables, twitchMessage.Message);
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not resolve purchase", e);
            }

            return true;
        }

        private static bool TryProcessSimpleIncident(Viewer viewer, ITwitchMessage twitchMessage, string query)
        {
            if (!TryFindSimpleIncident(query, out StoreIncidentSimple incidentSimple))
            {
                return false;
            }

            try
            {
                Purchase_Handler.ResolvePurchaseSimple(viewer, twitchMessage, incidentSimple, twitchMessage.Message);
            }
            catch (Exception e)
            {
                TkUtils.Logger.Error("Could not resolve purchase", e);
            }

            return true;
        }

        private static bool TryFindVariableIncident(string query, [CanBeNull] out StoreIncidentVariables incidentVariables)
        {
            incidentVariables = Purchase_Handler.allStoreIncidentsVariables.Find(i => CheckIncident(i, query));

            return incidentVariables != null;
        }

        private static bool TryFindSimpleIncident(string query, [CanBeNull] out StoreIncidentSimple incidentSimple)
        {
            incidentSimple = Purchase_Handler.allStoreIncidentsSimple.Find(i => CheckIncident(i, query));

            return incidentSimple != null;
        }

        private static bool CheckIncident([NotNull] StoreIncident incident, string query)
        {
            if (incident.defName.Equals("Item"))
            {
                return string.Equals(incident.abbreviation, query, StringComparison.InvariantCultureIgnoreCase);
            }

            return incident.cost > 0 && string.Equals(incident.abbreviation, query, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
