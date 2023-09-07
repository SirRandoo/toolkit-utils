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
using System.Reflection;
using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using TwitchToolkit;
using TwitchToolkit.PawnQueue;
using Verse;

namespace SirRandoo.ToolkitUtils.Patches;

[HarmonyPatch]
[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
internal static class ColonistUnnamedAlertPatch
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(Alert_UnnamedColonist), nameof(Alert_UnnamedColonist.GetReport));
    }

    private static Exception? Cleanup(MethodBase original, Exception? exception)
    {
        if (exception == null)
        {
            return null;
        }

        TkUtils.Logger.Error($"Could not patch {original.FullDescription()} -- Things will not work properly!", exception.InnerException ?? exception);

        return null;
    }

    /// <summary>
    ///     A Harmony patch for adjusting how the "Colonists need names"
    ///     in-game alert determines candidates. By default, the alert
    ///     selects all pawns that don't have a viewer assigned to them,
    ///     including borrowed pawns. This patch changes that functionality
    ///     to only select pawns that meet the following criteria: <br/>
    ///     <ul>
    ///         <li>The pawn isn't borrowed from any faction</li>
    ///         <li>The pawn isn't undead, if A RimWorld of Magic is active</li>
    ///         <li>The pawn isn't currently assigned to someone else</li>
    ///     </ul>
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [SuppressMessage("ReSharper", "RedundantAssignment")]
    private static bool Prefix(ref AlertReport __result)
    {
        __result = false;

        if (!ToolkitSettings.ViewerNamedColonistQueue)
        {
            return false;
        }

        var component = Current.Game.GetComponent<GameComponentPawns>();

        if (component == null)
        {
            return false;
        }

        Dictionary<string, Pawn> pawnHistory = component.pawnHistory;
        List<Pawn>? colonistsSpawned = Find.CurrentMap?.mapPawns.FreeColonistsSpawned;

        if (colonistsSpawned is null || colonistsSpawned.Count == pawnHistory.Count)
        {
            return false;
        }

        var container = new List<Pawn>();

        for (var i = 0; i < colonistsSpawned.Count; i++)
        {
            Pawn pawn = colonistsSpawned[i];

            if (pawnHistory.ContainsKey(pawn.LabelShort) || pawn.IsBorrowedByAnyFaction() || CompatRegistry.Magic?.IsUndead(pawn) == true)
            {
                continue;
            }

            container.Add(pawn);
        }

        if (container.Count > 0)
        {
            __result = AlertReport.CulpritsAre(container);
        }

        return false;
    }
}
