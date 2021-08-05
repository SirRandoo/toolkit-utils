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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using HarmonyLib;
using JetBrains.Annotations;
using SirRandoo.ToolkitUtils.Helpers;
using SirRandoo.ToolkitUtils.Models;
using SirRandoo.ToolkitUtils.Workers;
using ToolkitCore;
using ToolkitCore.Utilities;
using TwitchLib.Client.Models.Interfaces;
using TwitchToolkit;
using TwitchToolkit.Incidents;
using TwitchToolkit.Store;
using Verse;

namespace SirRandoo.ToolkitUtils.Harmony
{
    [HarmonyPatch(typeof(Purchase_Handler))]
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public static class PurchaseHandlerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("ResolvePurchase")]
        public static bool ResolvePurchasePrefix(
            Viewer viewer,
            [NotNull] ITwitchMessage twitchMessage,
            bool separateChannel = false
        )
        {
            var worker = ArgWorker.CreateInstance(CommandFilter.Parse(twitchMessage.Message).Skip(1));
            List<string> segments = CommandFilter.Parse(twitchMessage.Message).ToList();
            string query = segments.Skip(1).FirstOrFallback("");

            if (segments.Count < 2)
            {
                return false;
            }

        #if DEBUG
            if (viewer.username.Equals("sirrandoo") && query.Equals("storedebug"))
            {
                TkSettings.DebuggingIncidents = true;
                DebugIncidents(viewer, twitchMessage);
                return false;
            }
        #endif

            if (TryFindSimpleIncident(query, out StoreIncidentSimple incidentSimple))
            {
                try
                {
                    Purchase_Handler.ResolvePurchaseSimple(
                        viewer,
                        twitchMessage,
                        incidentSimple,
                        twitchMessage.Message
                    );
                }
                catch (Exception e)
                {
                    LogHelper.Error("Could not resolve purchase", e);
                }

                return false;
            }

            if (TryFindVariableIncident(query, out StoreIncidentVariables incidentVariables))
            {
                try
                {
                    Purchase_Handler.ResolvePurchaseVariables(
                        viewer,
                        twitchMessage,
                        incidentVariables,
                        twitchMessage.Message
                    );
                }
                catch (Exception e)
                {
                    LogHelper.Error("Could not resolve purchase", e);
                }

                return false;
            }

            Helper.Log($"abr: {query} ");
            if (!worker.TryGetNextAsItem(out ArgWorker.ItemProxy _))
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

            try
            {
                Purchase_Handler.ResolvePurchaseVariables(
                    viewer,
                    twitchMessage,
                    StoreIncidentDefOf.Item,
                    string.Join(" ", segments.ToArray())
                );
            }
            catch (Exception e)
            {
                LogHelper.Error("Could not resolve purchase", e);
            }

            return false;
        }

        private static void DebugIncidents(Viewer viewer, ITwitchMessage message)
        {
            var builder = new StringBuilder();
            foreach (StoreIncident incident in DefDatabase<StoreIncident>.AllDefs)
            {
                try
                {
                    Purchase_Handler.viewerNamesDoingVariableCommands.Clear();

                    switch (incident)
                    {
                        case StoreIncidentSimple simple:
                            Purchase_Handler.ResolvePurchaseSimple(
                                viewer,
                                message,
                                simple,
                                $"!buy {simple.abbreviation}"
                            );
                            continue;
                        case StoreIncidentVariables variables:
                            Purchase_Handler.ResolvePurchaseVariables(
                                viewer,
                                message,
                                variables,
                                $"!buy {variables.abbreviation} {variables.minPointsToFire}"
                            );
                            break;
                    }

                    builder.Append($" - {incident.abbreviation} (SUCCESS)");
                }
                catch (Exception e)
                {
                    builder.Append($" - {incident.abbreviation} (FAILED) -- {e.Message} ({e.GetType().Name})");
                }

                builder.Append("\n");
            }

            LogHelper.Info($"Status report:\n{builder}");
            TkSettings.DebuggingIncidents = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ResolvePurchaseSimple")]
        public static bool ResolvePurchaseSimplePrefix(
            [NotNull] Viewer viewer,
            ITwitchMessage twitchMessage,
            StoreIncidentSimple incident,
            string formattedMessage
        )
        {
            if (Purchase_Handler.CheckIfViewerIsInVariableCommandList(viewer.username)
                || !Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, incident.cost)
                || Purchase_Handler.CheckIfKarmaTypeIsMaxed(incident, viewer.username)
                || Purchase_Handler.CheckIfIncidentIsOnCooldown(incident, viewer.username))
            {
                return false;
            }

            if (!TryMakeIncident(incident, viewer, formattedMessage, out IncidentHelper inc))
            {
                LogHelper.Warn(@$"The incident ""{incident.defName}"" does not define an incident helper");
                return false;
            }

            if (!inc.IsPossible())
            {
                MessageHelper.ReplyToUser(viewer.username, "TwitchToolkitEventNotPossible".Localize());
                return false;
            }

            if (!ToolkitSettings.UnlimitedCoins)
            {
                viewer.TakeViewerCoins(incident.cost);
            }

            viewer.CalculateNewKarma(incident.karmaType, incident.cost);
            var comp = Current.Game.GetComponent<Store_Component>();
            var coordinator = Current.Game.GetComponent<Coordinator>();

            coordinator.QueueIncident(new IncidentProxy { SimpleIncident = inc });
            comp.LogIncident(incident);
            Store_Logger.LogPurchase(viewer.username, twitchMessage.Message);

            if (!ToolkitSettings.PurchaseConfirmations)
            {
                return false;
            }

            TwitchWrapper.SendChatMessage(
                Helper.ReplacePlaceholder(
                    "TwitchToolkitEventPurchaseConfirm".Localize(),
                    viewer: viewer.username,
                    first: incident.label.CapitalizeFirst()
                )
            );
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("ResolvePurchaseVariables")]
        public static bool ResolvePurchaseVariablesPrefix(
            [NotNull] Viewer viewer,
            ITwitchMessage twitchMessage,
            StoreIncidentVariables incident,
            string formattedMessage
        )
        {
            if (Purchase_Handler.CheckIfViewerIsInVariableCommandList(viewer.username)
                || !Purchase_Handler.CheckIfViewerHasEnoughCoins(viewer, incident.cost))
            {
                return false;
            }

            if (incident != StoreIncidentDefOf.Item
                && (Purchase_Handler.CheckIfKarmaTypeIsMaxed(incident, viewer.username)
                    && incident != IncidentDefOf.Sanctuary
                    || Purchase_Handler.CheckIfIncidentIsOnCooldown(incident, viewer.username)))
            {
                return false;
            }

            if (incident == StoreIncidentDefOf.Item && Purchase_Handler.CheckIfCarePackageIsOnCooldown(viewer.username))
            {
                return false;
            }

            if (!TryMakeIncident(incident, viewer, formattedMessage, out IncidentHelperVariables inc))
            {
                LogHelper.Warn(@$"The incident ""{incident.defName}"" does not define an incident helper");
                return false;
            }

            Purchase_Handler.viewerNamesDoingVariableCommands.Add(viewer.username.ToLowerInvariant());

            if (!inc.IsPossible(formattedMessage, viewer))
            {
                Purchase_Handler.viewerNamesDoingVariableCommands.Remove(viewer.username.ToLowerInvariant());
                return false;
            }

            var comp = Current.Game.GetComponent<Store_Component>();
            var coordinator = Current.Game.GetComponent<Coordinator>();

            coordinator.QueueIncident(new IncidentProxy { VariablesIncident = inc });
            Store_Logger.LogPurchase(viewer.username, twitchMessage.Message);
            comp.LogIncident(incident);
            return false;
        }

        [HarmonyPrefix]
        [HarmonyPatch("CheckIfViewerIsInVariableCommandList")]
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public static bool CheckIfViewerIsInVariableCommandListPrefix([NotNull] string username, ref bool __result)
        {
            if (!Purchase_Handler.viewerNamesDoingVariableCommands.Contains(username.ToLower()))
            {
                __result = false;
                return false;
            }

            __result = true;
            MessageHelper.ReplyToUser(username, "TKUtils.PausedExtended".LocalizeKeyed(CommandDefOf.UnstickMe.command));
            return false;
        }

        [ContractAnnotation("=> true,incidentHelper:notnull; => false,incidentHelper:null")]
        private static bool TryMakeIncident(
            StoreIncidentSimple incident,
            Viewer viewer,
            string message,
            out IncidentHelper incidentHelper
        )
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

        [ContractAnnotation("=> true,incidentHelper:notnull; => false,incidentHelper:null")]
        private static bool TryMakeIncident(
            StoreIncidentVariables incident,
            Viewer viewer,
            string message,
            out IncidentHelperVariables incidentHelper
        )
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
